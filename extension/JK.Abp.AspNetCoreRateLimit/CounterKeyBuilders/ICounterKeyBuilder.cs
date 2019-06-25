using JK.Abp.AspNetCoreRateLimit.Models;

namespace JK.Abp.AspNetCoreRateLimit.CounterKeyBuilders
{
    public interface ICounterKeyBuilder
    {
        string Build(ClientRequestIdentity requestIdentity, RateLimitRule rule);
    }
}