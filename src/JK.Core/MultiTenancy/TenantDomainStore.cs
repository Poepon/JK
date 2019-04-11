using Abp.Dependency;

namespace JK.MultiTenancy
{
    public class TenantDomainStore : ITenantDomainStore, ITransientDependency
    {
        private readonly ITenantDomainCache tenantDomainCache;

        public TenantDomainStore(ITenantDomainCache tenantDomainCache)
        {
            this.tenantDomainCache = tenantDomainCache;
        }

        public TenantDomain GetOrNull(string host, int port)
        {
            return tenantDomainCache.GetOrNull(host, port);
        }
    }
}
