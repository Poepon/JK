using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using JK.Abp.AspNetCoreRateLimit.Middleware;
using Microsoft.AspNetCore.Http;

namespace JK.Abp.AspNetCoreRateLimit
{
    public class JKAspNetCoreRateLimitModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterIfNot<IHttpContextAccessor, HttpContextAccessor>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<IRateLimitConfiguration, RateLimitConfiguration>(DependencyLifeStyle.Singleton);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JKAspNetCoreRateLimitModule).GetAssembly());
        }
    }

}
