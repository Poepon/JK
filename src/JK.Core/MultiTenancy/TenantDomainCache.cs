using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;

namespace JK.MultiTenancy
{
    public class TenantDomainCache : ITenantDomainCache,
      IEventHandler<EntityChangedEventData<TenantDomain>>
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<TenantDomain> _tenantDomainRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public TenantDomainCache(ICacheManager cacheManager,
             IRepository<TenantDomain> tenantDomainRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _cacheManager = cacheManager;
            _tenantDomainRepository = tenantDomainRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public TenantDomain GetOrNull(string host, int port)
        {
            return _cacheManager.GetCache(nameof(TenantDomain)).Get<string, TenantDomain>($"{host}:{port}",
                () =>
                {
                    return GetTenantDomainOrNull(host, port);
                }
                );
        }

        public void HandleEvent(EntityChangedEventData<TenantDomain> eventData)
        {
            _cacheManager.GetCache(nameof(TenantDomain)).Set($"{eventData.Entity.Host}:{eventData.Entity.Port}", eventData.Entity);
        }

        [UnitOfWork]
        protected virtual TenantDomain GetTenantDomainOrNull(string host, int port)
        {
            using (_unitOfWorkManager.Current.SetTenantId(null))
            {
                return _tenantDomainRepository.FirstOrDefault(d => d.Host == host && d.Port == port);
            }
        }
    }

}
