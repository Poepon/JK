using Abp.Modules;
using Abp.Reflection.Extensions;
using System;

namespace Abp.Serialization
{
    public class AbpSerializationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpSerializationModule).GetAssembly());
        }
    }
}
