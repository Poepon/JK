using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Abp.Exceptionless
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpCastleExceptionlessModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpCastleExceptionlessModule).GetAssembly());
        }
    }

}
