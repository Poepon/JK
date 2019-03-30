using System.Linq;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;

namespace JK.MultiTenancy
{
    public class TenantDomainCache : ITenantDomainCache,
        IEventHandler<EntityChangedEventData<TenantDomain>>
    {
        private readonly IRepository<TenantDomain> _domainRepository;
        private readonly ICacheManager _cacheManager;

        public TenantDomainCache(IRepository<TenantDomain> domainRepository,
            ICacheManager cacheManager)
        {
            _domainRepository = domainRepository;
            _cacheManager = cacheManager;
        }

        public TenantDomain GetOrNull(string host, int port)
        {
            return _cacheManager.GetCache(nameof(TenantDomain)).Get($"{host}:{port}",
                () => _domainRepository.GetAll().FirstOrDefault(d => d.Host == host && d.Port == port));
        }

        public void HandleEvent(EntityChangedEventData<TenantDomain> eventData)
        {
            _cacheManager.GetCache(nameof(TenantDomain)).Remove($"{eventData.Entity.Host}:{eventData.Entity.Port}");
        }

    }
}