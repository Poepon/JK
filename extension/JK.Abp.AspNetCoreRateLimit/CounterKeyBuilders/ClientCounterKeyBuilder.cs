using JK.Abp.AspNetCoreRateLimit.Models;

namespace JK.Abp.AspNetCoreRateLimit.CounterKeyBuilders
{
    public class ClientCounterKeyBuilder : ICounterKeyBuilder
    {
        public string Build(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            return $"{RateLimitOptions.RateLimitCounterPrefix}_{requestIdentity.ClientId}_{rule.Period}";
        }
    }
}