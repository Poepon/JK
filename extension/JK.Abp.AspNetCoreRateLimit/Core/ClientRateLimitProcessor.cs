using Abp.Runtime.Caching;
using JK.Abp.AspNetCoreRateLimit.CounterKeyBuilders;
using JK.Abp.AspNetCoreRateLimit.Middleware;
using JK.Abp.AspNetCoreRateLimit.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JK.Abp.AspNetCoreRateLimit.Core
{
    public class ClientRateLimitProcessor : RateLimitProcessor, IRateLimitProcessor
    {
        private readonly ClientRateLimitOptions _options;

        public ClientRateLimitProcessor(
           ClientRateLimitOptions options,
           ICacheManager cacheManager,
           IRateLimitConfiguration config)
        : base(cacheManager, options, new ClientCounterKeyBuilder(), config)
        {
            _options = options;
        }

        public async Task<IEnumerable<RateLimitRule>> GetMatchingRulesAsync(ClientRequestIdentity identity, CancellationToken cancellationToken = default)
        {
            var policy = await _cacheManager.GetCache(nameof(ClientRateLimitPolicy))
                    .GetOrDefaultAsync<string, ClientRateLimitPolicy>($"{_options.ClientPolicyPrefix}_{identity.ClientId}");

            return GetMatchingRules(identity, policy?.Rules);
        }
    }
}