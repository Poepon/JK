using Abp.Runtime.Caching;
using JK.Abp.AspNetCoreRateLimit.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace JK.Abp.AspNetCoreRateLimit.Store
{
    public class IpPolicyStore : IIpPolicyStore
    {
        private readonly ICacheManager _cacheManager;
        private readonly IpRateLimitOptions _options;
        private readonly IpRateLimitPolicies _policies;

        public IpPolicyStore(
            ICacheManager cacheManager,
            IOptions<IpRateLimitOptions> options = null,
            IOptions<IpRateLimitPolicies> policies = null)
        {
            _cacheManager = cacheManager;
            _options = options?.Value;
            _policies = policies?.Value;
        }

        public async Task SeedAsync()
        {
            // on startup, save the IP rules defined in appsettings
            if (_options != null && _policies != null)
            {
                await _cacheManager.GetCache(nameof(IpRateLimitPolicies)).SetAsync(IpRateLimitOptions.IpPolicyPrefix, _policies).ConfigureAwait(false);
            }
        }
    }
}