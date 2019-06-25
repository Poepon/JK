using System.Collections.Generic;

namespace JK.Abp.AspNetCoreRateLimit.Models
{
    public class ClientRateLimitPolicies
    {
        public List<ClientRateLimitPolicy> ClientRules { get; set; }
    }
}