using Abp.AspNetCore;
using Abp.Modules;
using Abp.Reflection.Extensions;
using JK.EntityFrameworkCore;

namespace JK.Web.Alliance
{
    [DependsOn(
         typeof(JKApplicationModule),
         typeof(JKEntityFrameworkModule),
         typeof(AbpAspNetCoreModule)
     )]
    public class JKWebAllianceModule : AbpModule
    {
        public JKWebAllianceModule(JKEntityFrameworkModule jKEntityFrameworkModule)
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
            IocManager.RegisterAssemblyByConvention(typeof(JKWebAllianceModule).GetAssembly());
        }
    }
}
