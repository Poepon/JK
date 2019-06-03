using Abp.Modules;
using Abp.Reflection.Extensions;
using JK.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Volo.Abp.RabbitMQ;

namespace JK.Web.Startup
{
    [DependsOn(typeof(JKWebCoreModule), typeof(AbpRabbitMqModule))]
    public class JKWebMvcModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public JKWebMvcModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Navigation.Providers.Add<AppXmlNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JKWebMvcModule).GetAssembly());
        }

    }
}
