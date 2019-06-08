using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using JK.Books;
using JK.Books.Crawlers;
using JK.Books.Workers;
using JK.Books.Workers.QiDian;
using JK.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Volo.Abp.RabbitMQ;

namespace JK.Web.Startup
{
    [DependsOn(typeof(JKWebCoreModule), typeof(AbpRabbitMqModule))]
    public class JKWebBookModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public JKWebBookModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JKWebBookModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.RegisterIfNot<IHtmlCompress, NullHtmlCompress>(DependencyLifeStyle.Transient);
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();

            workManager.Add(IocManager.Resolve<QiDianBookEndCreateWorker>());
            workManager.Add(IocManager.Resolve<QiDianBookIngCreateWorker>());
            workManager.Add(IocManager.Resolve<QiDianBookIngUpdateWorker>());
            workManager.Add(IocManager.Resolve<QiDianBookEndUpdateWorker>());

            workManager.Add(IocManager.Resolve<QiDianNewBookCreateWorker>());
            workManager.Add(IocManager.Resolve<QiDianBookAllCreateWorker>());


            workManager.Add(IocManager.Resolve<ChapterBookEndCrawlerWorker>());
            workManager.Add(IocManager.Resolve<ChapterSerializingCrawlerWorker>());

            workManager.Add(IocManager.Resolve<BookChapterContentCrawlerWorker>());
            workManager.Add(IocManager.Resolve<QiDianBookChapterContentCrawlerWorker>());

            workManager.Add(IocManager.Resolve<BookLinkCrawlerWorker>());

            workManager.Add(IocManager.Resolve<BookChapterLinkCrawlerWorker>());
        }

    }
}
