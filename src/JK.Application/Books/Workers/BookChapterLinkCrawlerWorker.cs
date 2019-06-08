using Abp.Dependency;
using Abp.Threading.Timers;
using Abp.BackgroundJobs;
using JK.Books.Jobs;
using Microsoft.Extensions.Options;
using JK.Configuration;

namespace JK.Books.Workers
{
    public class BookChapterLinkCrawlerWorker : PoeponPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IIocResolver iocResolver;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly WorkerConfig _workerConfig;
        public BookChapterLinkCrawlerWorker(
            AbpTimer timer,
            IIocResolver iocResolver,
            IOptions<WorkerConfig> options,
            IBackgroundJobManager backgroundJobManager) : base(timer)
        {
            this.iocResolver = iocResolver;
            _backgroundJobManager = backgroundJobManager;
            _workerConfig = options.Value;
            Timer.Period = _workerConfig.ChapterLinkWorkerPeriod * 60000;
            Timer.RunOnStart = _workerConfig.ChapterLinkWorkerEnable;
        }
        public virtual int CheckPeriodAsMilliseconds
        {
            get { return 30 * 1000 * 60; }
        }

        protected override void DoSomething()
        {
           
            if (!_workerConfig.ChapterLinkWorkerEnable)
                return;
            var crawlers = iocResolver.ResolveAll<IBookCrawler>();
            foreach (var crawler in crawlers)
            {
                //_backgroundJobManager.Enqueue<AllChapterLinkCrawlJob, AllChapterLinkCrawlJobArgs>(new AllChapterLinkCrawlJobArgs
                //{
                //    BookSource = crawler.Source
                //}, BackgroundJobPriority.AboveNormal);
            }
           
        }
    }
}