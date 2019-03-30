using Abp.Dependency;
using Abp.MultiTenancy;
using Microsoft.AspNetCore.Http;

namespace JK.MultiTenancy
{
    public class MyDomainTenantResolveContributor : ITenantResolveContributor, ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantDomainCache _tenantDomainCache;

        public MyDomainTenantResolveContributor(
            IHttpContextAccessor httpContextAccessor,
            ITenantDomainCache tenantDomainCache)
        {
            this._httpContextAccessor = httpContextAccessor;
            _tenantDomainCache = tenantDomainCache;
        }

        public int? ResolveTenantId()
        {
            HttpContext httpContext = this._httpContextAccessor.HttpContext;
            if (httpContext == null)
                return new int?();
            var host = httpContext.Request.Host.Host;
            var port = httpContext.Request.Host.Port;
            var tenantInfo = this._tenantDomainCache.GetOrNull(host, port.GetValueOrDefault());
            if (tenantInfo != null)
            {
                return new int?(tenantInfo.TenantId);
            }
            return new int?();
        }
    }
}