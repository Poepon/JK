using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using JK.Abp.RedisCache.Events.AutoSubscribe;
using JK.Abp.Serialization;

namespace JK.Abp.RedisCache.Events
{
    [DependsOn(typeof(AbpRedisCacheModule),typeof(AbpSerializationModule))]
    public class AbpRedisCacheEventsModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpRedisCacheEventsModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AutoRedisConsumer>().Subscribe();
        }

        public override void Shutdown()
        {
            IocManager.Resolve<AutoRedisConsumer>().Unsubscribe();
        }
    }
}
