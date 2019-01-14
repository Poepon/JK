using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using JK.Authorization;

namespace JK
{
    [DependsOn(
        typeof(JKCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class JKApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<JKAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(JKApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
