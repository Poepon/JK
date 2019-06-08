using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Threading.Timers;
using JK.Books.Jobs;
using JK.Configuration;
using Microsoft.Extensions.Options;

namespace JK.Books.Workers
{
    public class BookLinkCrawlerWorker : PoeponPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IIocResolver iocResolver;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly WorkerConfig _workerConfig;
        public virtual int CheckPeriodAsMilliseconds
        {
            get { return 24 * 60 * 60 * 1000; }
        }
        public BookLinkCrawlerWorker(AbpTimer timer,
            IIocResolver iocResolver,
            IOptions<WorkerConfig> options,
            IBackgroundJobManager backgroundJobManager) : base(timer)
        {
            this.iocResolver = iocResolver;
            _backgroundJobManager = backgroundJobManager;
            _workerConfig = options.Value;
            Timer.Period = _workerConfig.BookLinkWorkerPeriod * 60000;
            Timer.RunOnStart = _workerConfig.BookLinkWorkerEnable;
        }

        protected override void DoSomething()
        {
            if (!_workerConfig.BookLinkWorkerEnable)
                return;
            var crawlers = iocResolver.ResolveAll<IBookCrawler>();
            foreach (var crawler in crawlers)
            {
                //_backgroundJobManager.Enqueue<AllBookLinkCrawlJob, AllBookLinkCrawlJobArgs>
                //    (new AllBookLinkCrawlJobArgs
                //    {
                //        BookSource = crawler.Source
                //    });
            }
        }
    }
}