using System.Net.Http;
using Abp.BackgroundJobs;
using Abp.Threading.Timers;
using JK.Books.Interfaces;

namespace JK.Books.Workers.QiDian
{
    /// <summary>
    /// 完本小说爬虫
    /// </summary>
    public class QiDianBookEndUpdateWorker: QiDianBookCrawlerWorker
    {
        public QiDianBookEndUpdateWorker(AbpTimer timer,
            IBookAppService bookAppService,
            IBookLinkAppService bookLinkAppService,
            IHttpClientFactory httpClientFactory,
            IBackgroundJobManager backgroundJobManager) :
            base(timer, bookAppService, bookLinkAppService, httpClientFactory, backgroundJobManager)
        {
        }
        protected override bool CreateOrUpdate { get; set; } = false;

        protected override 排序方式 排序 { get; set; } = 排序方式.完本时间;

        protected override BookStatus? 完本状态 { get; set; } = BookStatus.BookEnd;

        protected override 显示方式 显示 { get; set; } = 显示方式.列表方式;

        protected override 更新时间 最后更新 { get; set; } = 更新时间.三日内;

        public override int CheckPeriodAsMilliseconds => 1000 * 60 * 5;
    }
}
