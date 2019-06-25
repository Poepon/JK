using System.Linq;
using Microsoft.AspNetCore.Http;

namespace JK.Abp.AspNetCoreRateLimit.Resolvers
{
    public class ClientHeaderResolveContributor : IClientResolveContributor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _headerName;

        public ClientHeaderResolveContributor(
            IHttpContextAccessor httpContextAccessor,
            string headerName)
        {
            _httpContextAccessor = httpContextAccessor;
            _headerName = headerName;
        }
        public string ResolveClient()
        {
            var clientId = "anon";
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext.Request.Headers.TryGetValue(_headerName, out var values))
            {
                clientId = values.First();
            }

            return clientId;
        }
    }
}