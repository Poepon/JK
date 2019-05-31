using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Volo.Abp.RabbitMQ
{   
    [DependsOn(typeof(AbpKernelModule))]
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
        public override void Shutdown()
        {
            IocManager.Resolve<IChannelPool>()
              .Dispose();

            IocManager.Resolve<IConnectionPool>()
                .Dispose();
        }
    }
}
