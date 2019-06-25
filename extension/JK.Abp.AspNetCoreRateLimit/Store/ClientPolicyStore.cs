using Abp.Runtime.Caching;
using JK.Abp.AspNetCoreRateLimit.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace JK.Abp.AspNetCoreRateLimit.Store
{
    public class ClientPolicyStore : IClientPolicyStore
    {
        private readonly ICacheManager _cacheManager;
        private readonly ClientRateLimitOptions _options;
        private readonly ClientRateLimitPolicies _policies;

        public ClientPolicyStore(
            ICacheManager cacheManager,
            IOptions<ClientRateLimitOptions> options = null,
            IOptions<ClientRateLimitPolicies> policies = null)
        {
            _cacheManager = cacheManager;
            _options = options?.Value;
            _policies = policies?.Value;
        }

        public async Task SeedAsync()
        {
            // on startup, save the IP rules defined in appsettings
            if (_options != null && _policies?.ClientRules != null)
            {
                var cache = _cacheManager.GetCache(nameof(ClientRateLimitPolicy));
                foreach (var rule in _policies.ClientRules)
                {
                    await cache.SetAsync($"{_options.ClientPolicyPrefix}_{rule.ClientId}",
                              new ClientRateLimitPolicy { ClientId = rule.ClientId, Rules = rule.Rules })
                          .ConfigureAwait(false);
                }
            }
        }
    }
}