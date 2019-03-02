using Abp.Modules;
using Abp.Reflection.Extensions;
using JK.Configuration;
using JK.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace JK.Web.Public
{
    [DependsOn(typeof(JKWebCoreModule))]
    public class JKWebPublicModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public JKWebPublicModule(IHostingEnvironment env, JKEntityFrameworkModule jKEntityFrameworkModule)
        {
            _env = env;
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment()); ;
            jKEntityFrameworkModule.SkipDbSeed = true;
            jKEntityFrameworkModule.SkipDbContextRegistration = false;
        }

        public override void PostInitialize()
        {
            this.Configuration.Auditing.IsEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JKWebPublicModule).GetAssembly());
        }
    }
}
