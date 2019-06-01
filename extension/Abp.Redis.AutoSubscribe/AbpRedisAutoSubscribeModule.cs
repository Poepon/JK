using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Serialization;
using System;

namespace Abp.Redis
{
    [DependsOn(typeof(AbpKernelModule),typeof(AbpSerializationModule))]
    public class AbpRedisAutoSubscribeModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<AbpRedisOptions>(Dependency.DependencyLifeStyle.Singleton);
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
