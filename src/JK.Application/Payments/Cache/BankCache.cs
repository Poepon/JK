using Abp.Domain.Entities.Caching;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using JK.Payments.Bacis;
using JK.Payments.Dto;
using System.Collections.Generic;
using System.Linq;
using Abp.Events.Bus.Entities;

namespace JK.Payments.Cache
{
    public class BankCache : EntityCache<Bank, BankDto>, IBankCache
    {
        public BankCache(ICacheManager cacheManager, IRepository<Bank, int> repository, string cacheName = "BankItem") : base(cacheManager, repository, cacheName)
        {
        }

        public BankDto Get(string code)
        {
            return CacheManager.GetCache<string, BankDto>("BankItemByCode").Get(code, () =>
            {
                var entity = Repository.GetAll().SingleOrDefault(b => b.BankCode == code);
                if (entity == null)
                    return null;
                return ObjectMapper.Map<BankDto>(entity);
            });
        }

        public IReadOnlyList<BankDto> GetAll()
        {
            return CacheManager.GetCache("BankList").Get("AllList", () =>
            {
                var entities = Repository.GetAllList();
                if (entities == null)
                    return null;
                return ObjectMapper.Map<List<BankDto>>(entities);
            });
        }

        public override void HandleEvent(EntityChangedEventData<Bank> eventData)
        {
            base.HandleEvent(eventData);

            CacheManager.GetCache("BankItemByCode").Remove(eventData.Entity.BankCode);
            CacheManager.GetCache("BankList").Remove("AllList");
        }
    }
}