using Abp.Runtime.Caching;
using JK.Abp.AspNetCoreRateLimit.Core;
using JK.Abp.AspNetCoreRateLimit.Models;
using JK.Abp.AspNetCoreRateLimit.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JK.Abp.AspNetCoreRateLimit.Middleware
{
    public class ClientRateLimitMiddleware : RateLimitMiddleware<ClientRateLimitProcessor>
    {
        private readonly ILogger<ClientRateLimitMiddleware> _logger;

        public ClientRateLimitMiddleware(RequestDelegate next,
            IOptions<ClientRateLimitOptions> options,
            ICacheManager cacheManager,
            IRateLimitConfiguration config,
            ILogger<ClientRateLimitMiddleware> logger)
        : base(next, options?.Value, new ClientRateLimitProcessor(options?.Value, cacheManager, config), config)
        {
            _logger = logger;
        }

        protected override void LogBlockedRequest(HttpContext httpContext, ClientRequestIdentity identity, RateLimitCounter counter, RateLimitRule rule)
        {
            _logger.LogInformation($"Request {identity.HttpVerb}:{identity.Path} from ClientId {identity.ClientId} has been blocked, quota {rule.Limit}/{rule.Period} exceeded by {counter.Count}. Blocked by rule {rule.Endpoint}, TraceIdentifier {httpContext.TraceIdentifier}.");
        }
    }
}