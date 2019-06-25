using JK.Abp.AspNetCoreRateLimit.Models;

namespace JK.Abp.AspNetCoreRateLimit.CounterKeyBuilders
{
    public class IpCounterKeyBuilder : ICounterKeyBuilder
    {
        public string Build(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            return $"{RateLimitOptions.RateLimitCounterPrefix}_{requestIdentity.ClientIp}_{rule.Period}";
        }
    }
}