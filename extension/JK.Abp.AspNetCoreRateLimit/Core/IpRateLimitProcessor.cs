using Abp.Runtime.Caching;
using JK.Abp.AspNetCoreRateLimit.CounterKeyBuilders;
using JK.Abp.AspNetCoreRateLimit.Middleware;
using JK.Abp.AspNetCoreRateLimit.Models;
using JK.Abp.AspNetCoreRateLimit.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JK.Abp.AspNetCoreRateLimit.Core
{
    public class IpRateLimitProcessor : RateLimitProcessor, IRateLimitProcessor
    {
        private readonly IpRateLimitOptions _options;

        public IpRateLimitProcessor(
           IpRateLimitOptions options,
           ICacheManager cacheManager,
           IRateLimitConfiguration config)
        : base(cacheManager, options, new IpCounterKeyBuilder(), config)
        {
            _options = options;
        }

        public async Task<IEnumerable<RateLimitRule>> GetMatchingRulesAsync(ClientRequestIdentity identity, CancellationToken cancellationToken = default)
        {
            var policies = await _cacheManager.GetCache(nameof(IpRateLimitPolicies))
                    .GetOrDefaultAsync<string, IpRateLimitPolicies>($"{IpRateLimitOptions.IpPolicyPrefix}");

            var rules = new List<RateLimitRule>();

            if (policies?.IpRules?.Any() == true)
            {
                // search for rules with IP intervals containing client IP
                var matchPolicies = policies.IpRules.Where(r => IpParser.ContainsIp(r.Ip, identity.ClientIp));

                foreach (var item in matchPolicies)
                {
                    rules.AddRange(item.Rules);
                }
            }

            return GetMatchingRules(identity, rules);
        }

        public override bool IsWhitelisted(ClientRequestIdentity requestIdentity)
        {
            if (_options.IpWhitelist != null && IpParser.ContainsIp(_options.IpWhitelist, requestIdentity.ClientIp))
            {
                return true;
            }

            return base.IsWhitelisted(requestIdentity);
        }
    }
}