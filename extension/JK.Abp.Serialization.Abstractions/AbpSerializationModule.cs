using Abp.Modules;
using Abp.Reflection.Extensions;

namespace JK.Abp.Serialization
{
    public class AbpSerializationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpSerializationModule).GetAssembly());
        }
    }
}
