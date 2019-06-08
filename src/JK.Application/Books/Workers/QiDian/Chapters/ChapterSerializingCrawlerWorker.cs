using Abp.BackgroundJobs;
using Abp.Threading.Timers;
using JK.Books.Interfaces;

namespace JK.Books.Workers.QiDian
{
    public class ChapterSerializingCrawlerWorker : QiDianBookChapterCrawlerWorker
    {
        public ChapterSerializingCrawlerWorker(AbpTimer timer, IBookAppService bookAppService, IBackgroundJobManager backgroundJobManager) : base(timer, bookAppService, backgroundJobManager)
        {
        }

        public override BookStatus BookStatus => BookStatus.Serializing;
    }
}