using System.Collections.Generic;

namespace JK.Abp.AspNetCoreRateLimit.Models
{
    public class IpRateLimitPolicies
    {
        public List<IpRateLimitPolicy> IpRules { get; set; } = new List<IpRateLimitPolicy>();
    }
}