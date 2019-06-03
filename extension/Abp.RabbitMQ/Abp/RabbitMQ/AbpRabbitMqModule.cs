using Abp;
using Abp.Modules;
using Abp.RabbitMQ.AutoSubscribe;
using Abp.Reflection.Extensions;
using Abp.Serialization;

namespace Volo.Abp.RabbitMQ
{
    [DependsOn(typeof(AbpKernelModule), typeof(AbpSerializationModule))]
    public class AbpRabbitMqModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<AbpRabbitMqOptions>();
        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpRabbitMqModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AutoRabbitMQConsumer>().Subscribe();
        }

        public override void Shutdown()
        {
            IocManager.Resolve<IChannelPool>()
              .Dispose();

            IocManager.Resolve<IConnectionPool>()
                .Dispose();
        }
    }
}
