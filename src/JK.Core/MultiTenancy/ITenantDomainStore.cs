namespace JK.MultiTenancy
{
    public interface ITenantDomainStore
    {
        TenantDomain GetOrNull(string host, int port);
    }
}
