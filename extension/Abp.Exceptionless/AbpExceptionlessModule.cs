using Abp.Events.Bus.Exceptions;
using Abp.Events.Bus.Handlers;
using Abp.Modules;

namespace Abp.Exceptionless
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpExceptionlessModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.Register<IAsyncEventHandler<AbpHandledExceptionData>, ExceptionlessHandler>(Dependency.DependencyLifeStyle.Transient);
        }
    }

}
