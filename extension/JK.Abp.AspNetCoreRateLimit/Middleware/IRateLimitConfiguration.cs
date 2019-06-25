using System;
using System.Collections.Generic;
using JK.Abp.AspNetCoreRateLimit.CounterKeyBuilders;
using JK.Abp.AspNetCoreRateLimit.Resolvers;

namespace JK.Abp.AspNetCoreRateLimit.Middleware
{
    public interface IRateLimitConfiguration
    {
        IList<IClientResolveContributor> ClientResolvers { get; }

        IList<IIpResolveContributor> IpResolvers { get; }

        ICounterKeyBuilder EndpointCounterKeyBuilder { get; }

        Func<double> RateIncrementer { get; }
    }
}