using Abp.Application.Services.Dto;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Threading.Timers;
using JK.Books.Interfaces;
using JK.Books.Jobs;
using JK.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace JK.Books.Workers
{
    public class QiDianBookChapterContentCrawlerWorker : BookChapterContentCrawlerWorker
    {
        public override bool IsFree => true;
        public QiDianBookChapterContentCrawlerWorker(AbpTimer timer,
            IOptions<WorkerConfig> options,
            IBookChapterAppService bookChapterAppService,
            IBackgroundJobManager backgroundJobManager) : base(timer, options, bookChapterAppService, backgroundJobManager)
        {
        }
    }
    /// <summary>
    /// 章节内容爬虫
    /// </summary>
    public class BookChapterContentCrawlerWorker : PoeponPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IBookChapterAppService _bookChapterAppService;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly WorkerConfig _workerConfig;
        public BookChapterContentCrawlerWorker(
            AbpTimer timer,
            IOptions<WorkerConfig> options,
            IBookChapterAppService bookChapterAppService,
            IBackgroundJobManager backgroundJobManager
            ) : base(timer)
        {
            _bookChapterAppService = bookChapterAppService;
            _backgroundJobManager = backgroundJobManager;
            _workerConfig = options.Value;

            Timer.Period = _workerConfig.ChapterContentWorkerPeriod * 60000;
            Timer.RunOnStart = _workerConfig.ChapterContentWorkerEnable;
        }

        public virtual bool IsFree => false;

        public virtual int CheckPeriodAsMilliseconds
        {
            get { return 1000 * 60 * 60; }
        }

        protected override void DoSomething()
        {
            if (!_workerConfig.ChapterContentWorkerEnable)
                return;
            try
            {
                Logger.Info("爬内容开始。");
                int pageIndex = 1;
                int pageSize = 100;
                var chapterCount = _bookChapterAppService.Count(c => c.IsFree == IsFree &&
                c.HasSrc == false &&
                c.ChapterLinks.Any());

                var pageCount = (int)Math.Ceiling(chapterCount * 1.0M / pageSize);
                Logger.Warn($"待处理图书目录{chapterCount}章。共{pageCount}页。");
                for (; pageIndex <= pageCount; pageIndex++)
                {
                    Logger.Warn($"共{pageCount}页。当前第{pageIndex}页。");
                    try
                    {
                        var data = _bookChapterAppService.GetAllId(c => c.IsFree == IsFree &&
                        c.HasSrc == false &&
                        c.ChapterLinks.Any(),
                            new PagedAndSortedResultRequestDto
                            {
                                MaxResultCount = pageSize,
                                Sorting = "Id Asc",
                                SkipCount = (pageIndex - 1) * pageSize
                            });

                        foreach (var item in data.Items)
                        {
                            //_backgroundJobManager.Enqueue<ChapterContentCrawlJob, ChapterContentCrawlJobArgs>(new ChapterContentCrawlJobArgs
                            //{
                            //    BookId = item.BookId,
                            //    ChapterId = item.ChapterId
                            //}, BackgroundJobPriority.High);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Warn("GetAllId", e);
                    }

                }
            }
            catch (Exception e)
            {
                Logger.Error("爬内容异常", e);
            }
            finally
            {
                Logger.Info($"爬内容结束。");
            }
        }
    }
}