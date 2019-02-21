using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JK.Web.CdnTagHelpers
{
    public static class CdnServiceCollectionExtensions
    {
        public static void AddCdn(this IServiceCollection services, CdnOptions cdnOptions)
        {
            services.Configure<CdnOptions>(option =>
            {
                option.Url = cdnOptions.Url;
                option.Prefetch = cdnOptions.Prefetch;
            });
        }

        public static void AddCdn(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CdnOptions>(configuration);
        }
    }
}
