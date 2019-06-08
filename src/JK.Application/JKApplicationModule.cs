using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using JK.Payments.Orders;

namespace JK
{
    [DependsOn(
        typeof(JKCoreModule),
        typeof(AbpAutoMapperModule))]
    public class JKApplicationModule : AbpModule
    {
        public override void PostInitialize()
        {
            IocManager.Resolve<IIdGenerator>().Init(0, 0);
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
