using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;
using JK.Abp.RabbitMQ.AutoSubscribe;
using JK.Abp.Serialization;

namespace JK.Abp.RabbitMQ
{
    [DependsOn(typeof(AbpSerializationModule))]
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
