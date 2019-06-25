namespace JK.Abp.AspNetCoreRateLimit.Models
{
    public class ClientRateLimitPolicy : RateLimitPolicy
    {
        public string ClientId { get; set; }
    }
}