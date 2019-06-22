using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;
using JK.Abp.Redis.AutoSubscribe;
using JK.Abp.Serialization;

namespace JK.Abp.Redis
{
    [DependsOn(typeof(AbpKernelModule),typeof(AbpSerializationModule))]
    public class AbpRedisAutoSubscribeModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<AbpRedisOptions>(global::Abp.Dependency.DependencyLifeStyle.Singleton);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpRedisAutoSubscribeModule).GetAssembly());
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
