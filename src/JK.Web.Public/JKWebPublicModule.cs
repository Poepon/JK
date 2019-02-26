using Abp.AspNetCore;
using Abp.Modules;
using Abp.Reflection.Extensions;
using JK.EntityFrameworkCore;

namespace JK.Web.Public
{
    [DependsOn(
       typeof(JKApplicationModule),
       typeof(JKEntityFrameworkModule),
       typeof(AbpAspNetCoreModule)
   )]
    public class JKWebPublicModule : AbpModule
    {
        public JKWebPublicModule(JKEntityFrameworkModule jKEntityFrameworkModule)
        {
            jKEntityFrameworkModule.SkipDbSeed = true;
            jKEntityFrameworkModule.SkipDbContextRegistration = true;
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
