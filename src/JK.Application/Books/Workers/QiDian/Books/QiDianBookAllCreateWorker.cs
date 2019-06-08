using System.Net.Http;
using Abp.BackgroundJobs;
using Abp.Threading.Timers;
using JK.Books.Interfaces;

namespace JK.Books.Workers.QiDian
{
    public class QiDianNewBookCreateWorker : QiDianBookCrawlerWorker
    {
        public QiDianNewBookCreateWorker(AbpTimer timer, IBookAppService bookAppService, IBookLinkAppService bookLinkAppService, IHttpClientFactory httpClientFactory, IBackgroundJobManager backgroundJobManager) : base(timer, bookAppService, bookLinkAppService, httpClientFactory, backgroundJobManager)
        {
        }

        protected override bool CreateOrUpdate { get; set; } = true;

        protected override 排序方式 排序 { get; set; } = 排序方式.更新时间;

        protected override BookStatus? 完本状态 { get; set; } = null;

        protected override 显示方式 显示 { get; set; } = 显示方式.栅格方式;

        protected override 更新时间 最后更新 { get; set; } = 更新时间.全部;

        protected override bool 只看三个月内新书 { get; set; } = true;
    }
    public class QiDianBookAllCreateWorker : QiDianBookCrawlerWorker
    {
        public QiDianBookAllCreateWorker(AbpTimer timer,
            IBookAppService bookAppService,
            IBookLinkAppService bookLinkAppService,
            IHttpClientFactory httpClientFactory,
            IBackgroundJobManager backgroundJobManager) :
            base(timer, bookAppService, bookLinkAppService, httpClientFactory, backgroundJobManager)
        {
        }

        protected override bool CreateOrUpdate { get; set; } = true;

        protected override 排序方式 排序 { get; set; } = 排序方式.总收藏;

        protected override BookStatus? 完本状态 { get; set; } = null;

        protected override 显示方式 显示 { get; set; } = 显示方式.栅格方式;

        protected override 更新时间 最后更新 { get; set; } = 更新时间.全部;

        protected override bool 只看三个月内新书 { get; set; } = false;

        public override int CheckPeriodAsMilliseconds => base.CheckPeriodAsMilliseconds * 6;

    }
}
