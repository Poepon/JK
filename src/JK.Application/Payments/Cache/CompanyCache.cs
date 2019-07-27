using Abp.Domain.Entities.Caching;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Runtime.Caching;
using JK.Payments.Integration;
using JK.Payments.Integration.Dto;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JK.Payments.Cache
{
    public class CompanyCache : EntityCache<Company, CompanyDto>, ICompanyCache
    {
        public CompanyCache(ICacheManager cacheManager, IRepository<Company, int> repository, string cacheName = null) : base(cacheManager, repository, cacheName)
        {
        }

        public IReadOnlyList<CompanyDto> GetActiveList()
        {
            return CacheManager.GetCache<string, List<CompanyDto>>("CompanyList").Get("ActiveList", () =>
            {
                var entities = Repository.GetAll().Where(c => c.IsActive).ToList();

                var list = ObjectMapper.Map<List<CompanyDto>>(entities);

                return list;
            });
        }

        public IReadOnlyList<CompanyDto> GetAll()
        {
            return CacheManager.GetCache<string, List<CompanyDto>>("CompanyList").Get("AllList", () =>
            {
                var entities = Repository.GetAllList();

                var list = ObjectMapper.Map<List<CompanyDto>>(entities);

                return list;
            });
        }

        public override void HandleEvent(EntityChangedEventData<Company> eventData)
        {
            base.HandleEvent(eventData);
            var typedCache = CacheManager.GetCache<string, List<CompanyDto>>("CompanyList");
            typedCache.Remove("ActiveList");
            typedCache.Remove("AllList");
        }
    }
}
