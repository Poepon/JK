using System.Collections.Generic;

namespace JK.Abp.AspNetCoreRateLimit.Models
{
    public class RateLimitPolicy
    {
        public List<RateLimitRule> Rules { get; set; } = new List<RateLimitRule>();
    }
}
