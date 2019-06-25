namespace JK.Abp.AspNetCoreRateLimit.Models
{
    public class IpRateLimitPolicy : RateLimitPolicy
    {
        public string Ip { get; set; }
    }
}