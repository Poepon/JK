using Abp.Dependency;

namespace JK.MultiTenancy
{
    public interface ITenantDomainCache : ITransientDependency
    {
        TenantDomain GetOrNull(string host, int port);
    }
}