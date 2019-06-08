using System;
using Abp.Application.Services.Dto;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Threading.Timers;
using JK.Books.Interfaces;
using JK.Books.Jobs;

namespace JK.Books.Workers.QiDian
{
    /// <summary>
    /// 起点图书章节爬虫
    /// </summary>
    public abstract class QiDianBookChapterCrawlerWorker : PoeponPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IBookAppService _bookAppService;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private const int CheckPeriodAsMilliseconds = 1000 * 60 * 10; //10 分钟

        public abstract BookStatus BookStatus { get; }

        public QiDianBookChapterCrawlerWorker(
            AbpTimer timer,
            IBookAppService bookAppService,
            IBackgroundJobManager backgroundJobManager) : base(timer)
        {
            _bookAppService = bookAppService;
            _backgroundJobManager = backgroundJobManager;
            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;
        }

        protected override void DoSomething()
        {
            try
            {
                Logger.Info($"爬{BookStatus}章节目录开始。");

                var count = _bookAppService.Count(b =>
                b.Status == BookStatus &&
                b.IsActive == true &&
                b.LastUpdateDateTime >= b.LastCrawlDateTime);
                int pageIndex = 1;
                int pageSize = 100;
                int pageCount = Convert.ToInt32(Math.Ceiling(count / (pageSize * 1.0m)));
                Logger.Info($"章节{pageCount}待抓取页数。");
                for (; pageIndex <= pageCount; pageIndex++)
                {
                    var input = new PagedAndSortedResultRequestDto()
                    {
                        MaxResultCount = pageSize,
                        SkipCount = (pageIndex - 1) * pageSize,
                        Sorting = "Id asc"
                    };
                    var result = _bookAppService.GetBooks(b => 
                    b.Status == BookStatus &&
                    b.IsActive == true &&
                    b.LastUpdateDateTime >= b.LastCrawlDateTime,
                        input);
                    foreach (var book in result.Items)
                    {
                        //_backgroundJobManager.Enqueue<QiDianChapterCrawlJob, QiDianChapterCrawlJobArgs>(new QiDianChapterCrawlJobArgs()
                        //{
                        //    BookId = book.Id
                        //});
                    }
                    Logger.Info($"采集{BookStatus}章节目录第{pageIndex}页成功。");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"爬{BookStatus}章节目录异常", e);
            }
            finally
            {
                Logger.Info($"爬{BookStatus}章节目录结束。");
            }
        }
    }
}