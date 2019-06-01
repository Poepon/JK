using Abp.AutoMapper;
using Abp.Modules;
using Abp.Redis;
using Abp.Reflection.Extensions;
using JK.Authorization;
using JK.Payments.Orders;
using JK.Redis.Test;

namespace JK
{
    [DependsOn(
        typeof(JKCoreModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpRedisAutoSubscribeModule))]
    public class JKApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<AppXmlAuthorizationProvider>();
        }
        public override void PostInitialize()
        {
            IocManager.Resolve<IIdGenerator>().Init(0, 0);
            var pub = IocManager.Resolve<IRedisProducer>();
            //pub.PublishAsync("Test1", "hi").Wait();
            pub.PublishAsync("Test2", new Message() { Text = "test" }).Wait();
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
