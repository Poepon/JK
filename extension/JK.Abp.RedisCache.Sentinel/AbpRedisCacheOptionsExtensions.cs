using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Caching.Redis;

namespace JK.Abp.RedisCache.Sentinel
{
    public static class AbpRedisCacheOptionsExtensions
    {
        public static void UseSentinel(this AbpRedisCacheOptions options)
        {
            options.AbpStartupConfiguration
                .ReplaceService<IAbpRedisCacheDatabaseProvider, AbpRedisCacheDatabaseProvider>(DependencyLifeStyle.Singleton);
        }
    }
}
