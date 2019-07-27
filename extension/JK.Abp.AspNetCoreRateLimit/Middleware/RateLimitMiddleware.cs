using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JK.Abp.AspNetCoreRateLimit.Core;
using JK.Abp.AspNetCoreRateLimit.Models;
using Microsoft.AspNetCore.Http;

namespace JK.Abp.AspNetCoreRateLimit.Middleware
{
    public abstract class RateLimitMiddleware<TProcessor>
        where TProcessor : IRateLimitProcessor
    {
        private readonly RequestDelegate _next;
        private readonly TProcessor _processor;
        private readonly RateLimitOptions _options;
        private readonly IRateLimitConfiguration _config;

        protected RateLimitMiddleware(
            RequestDelegate next,
            RateLimitOptions options,
            TProcessor processor,
            IRateLimitConfiguration config)
        {
            _next = next;
            _options = options;
            _processor = processor;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            // check if rate limiting is enabled
            if (_options == null)
            {
                await _next.Invoke(context);
                return;
            }

            var data = await Check(context);
            if (data.result)
            {
                await _next.Invoke(context);
            }
            else
            {
                // break execution
                await ReturnQuotaExceededResponse(context, data.message);
            }
        }

        async Task<(bool result, string message)> Check(HttpContext httpContext)
        {
            // compute identity from request
            var identity = ResolveIdentity(httpContext);

            // check white list
            if (!_processor.IsWhitelisted(identity))
            {
                var rules = await _processor.GetMatchingRulesAsync(identity, httpContext.RequestAborted);

                var rulesDict = new Dictionary<RateLimitRule, RateLimitCounter>();
                foreach (var rule in rules)
                {
                    // increment counter		
                    var rateLimitCounter =
                        await _processor.ProcessRequestAsync(identity, rule, httpContext.RequestAborted);

                    if (rule.Limit > 0)
                    {
                        // check if key expired
                        if (rateLimitCounter.Timestamp + rule.PeriodTimespan.Value < DateTime.UtcNow)
                        {
                            continue;
                        }

                        // check if limit is reached
                        if (rateLimitCounter.Count > rule.Limit)
                        {
                            //compute retry after value
                            var retryAfter = rateLimitCounter.Timestamp.RetryAfterFrom(rule);

                            // log blocked request
                            LogBlockedRequest(httpContext, identity, rateLimitCounter, rule);
                            var message = string.Format(
                                _options.QuotaExceededResponse?.Content ??
                                _options.QuotaExceededMessage ??
                                "API calls quota exceeded! maximum admitted {0} per {1}.", rule.Limit, rule.Period,
                                retryAfter);
                            if (!_options.DisableRateLimitHeaders)
                            {
                                httpContext.Response.Headers["Retry-After"] = retryAfter;
                            }

                            return (false, message);
                        }
                    }
                    // if limit is zero or less, block the request.
                    else
                    {
                        // log blocked request
                        LogBlockedRequest(httpContext, identity, rateLimitCounter, rule);
                        // break execution (Int32 max used to represent infinity)
                        var retryAfter = int.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        var message = string.Format(
                            _options.QuotaExceededResponse?.Content ??
                            _options.QuotaExceededMessage ??
                            "API calls quota exceeded! maximum admitted {0} per {1}.", rule.Limit, rule.Period,
                            retryAfter);
                        if (!_options.DisableRateLimitHeaders)
                        {
                            httpContext.Response.Headers["Retry-After"] = retryAfter;
                        }

                        return (false, message);
                    }

                    rulesDict.Add(rule, rateLimitCounter);
                }

                // set X-Rate-Limit headers for the longest period
                if (rulesDict.Any() && !_options.DisableRateLimitHeaders)
                {
                    var rule = rulesDict.OrderByDescending(x => x.Key.PeriodTimespan).FirstOrDefault();
                    var headers = _processor.GetRateLimitHeaders(rule.Value, rule.Key, httpContext.RequestAborted);

                    headers.Context = httpContext;

                    httpContext.Response.OnStarting(SetRateLimitHeaders, state: headers);
                }
            }

            return (true, "");
        }

        public virtual ClientRequestIdentity ResolveIdentity(HttpContext httpContext)
        {
            string clientIp = null;
            string clientId = null;

            if (_config.ClientResolvers?.Any() == true)
            {
                foreach (var resolver in _config.ClientResolvers)
                {
                    clientId = resolver.ResolveClient();

                    if (!string.IsNullOrEmpty(clientId))
                    {
                        break;
                    }
                }
            }

            if (_config.IpResolvers?.Any() == true)
            {
                foreach (var resolver in _config.IpResolvers)
                {
                    clientIp = resolver.ResolveIp();

                    if (!string.IsNullOrEmpty(clientIp))
                    {
                        break;
                    }
                }
            }

            return new ClientRequestIdentity
            {
                ClientIp = clientIp,
                Path = httpContext.Request.Path.ToString().ToLowerInvariant(),
                HttpVerb = httpContext.Request.Method.ToLowerInvariant(),
                ClientId = clientId
            };
        }

        public virtual Task ReturnQuotaExceededResponse(HttpContext httpContext, string message)
        {
            httpContext.Response.StatusCode = _options.QuotaExceededResponse?.StatusCode ?? _options.HttpStatusCode;
            httpContext.Response.ContentType = _options.QuotaExceededResponse?.ContentType ?? "text/plain";

            return httpContext.Response.WriteAsync(message);
        }

        protected abstract void LogBlockedRequest(HttpContext httpContext, ClientRequestIdentity identity, RateLimitCounter counter, RateLimitRule rule);

        private Task SetRateLimitHeaders(object rateLimitHeaders)
        {
            var headers = (RateLimitHeaders)rateLimitHeaders;

            headers.Context.Response.Headers["X-Rate-Limit-Limit"] = headers.Limit;
            headers.Context.Response.Headers["X-Rate-Limit-Remaining"] = headers.Remaining;
            headers.Context.Response.Headers["X-Rate-Limit-Reset"] = headers.Reset;

            return Task.CompletedTask;
        }
    }
}