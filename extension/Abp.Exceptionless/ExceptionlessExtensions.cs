using Exceptionless;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using LessExtensions = Exceptionless.ExceptionlessExtensions;

namespace Abp.Exceptionless
{
    public static class ExceptionlessExtensions
    {
        public static IApplicationBuilder UseExceptionless(this IApplicationBuilder app, string apiKey)
        {
            return LessExtensions.UseExceptionless(app, apiKey);
        }

        public static IApplicationBuilder UseExceptionless(this IApplicationBuilder app, ExceptionlessClient client = null)
        {
            return LessExtensions.UseExceptionless(app, client);
        }
        public static IApplicationBuilder UseExceptionless(this IApplicationBuilder app, IConfiguration configuration)
        {
            return LessExtensions.UseExceptionless(app, configuration);
        }
    }
}
