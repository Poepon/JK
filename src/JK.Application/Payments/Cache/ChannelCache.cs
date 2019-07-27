using Abp;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using JK.Payments.Bacis;
using JK.Payments.Dto;
using System.Collections.Generic;
using System.Linq;
using Abp.ObjectMapping;
using Abp.Domain.Entities.Caching;
using Abp.Events.Bus.Entities;

namespace JK.Payments.Cache
{
    public class ChannelCache : EntityCache<Channel, ChannelDto>, IChannelCache
    {
        public ChannelCache(ICacheManager cacheManager, IRepository<Channel, int> repository, string cacheName = "BankItem") : base(cacheManager, repository, cacheName)
        {
        }

        public ChannelDto Get(string code)
        {
            return CacheManager.GetCache<string, ChannelDto>("ChannelByCode").Get(code, () =>
            {
                var entity = Repository.GetAll().Where(c => c.IsActive).ToList();

                var dto = ObjectMapper.Map<ChannelDto>(entity);

                return dto;
            });
        }

        public IReadOnlyList<ChannelDto> GetActiveList()
        {
            return CacheManager.GetCache<string, List<ChannelDto>>("ChannelList").Get("ActiveList", () =>
            {
                var entities = Repository.GetAll().Where(c => c.IsActive).ToList();

                var list = ObjectMapper.Map<List<ChannelDto>>(entities);

                return list;
            });
        }

        public IReadOnlyList<ChannelDto> GetAll()
        {
            return CacheManager.GetCache<string, List<ChannelDto>>("ChannelList").Get("AllList", () =>
            {
                var entities = Repository.GetAllList();

                var list = ObjectMapper.Map<List<ChannelDto>>(entities);

                return list;
            });
        }

        public override void HandleEvent(EntityChangedEventData<Channel> eventData)
        {
            base.HandleEvent(eventData);
            CacheManager.GetCache<string, ChannelDto>("ChannelByCode").Remove(eventData.Entity.ChannelCode);
            CacheManager.GetCache<string, List<ChannelDto>>("ChannelList").Remove("ActiveList");
            CacheManager.GetCache<string, List<ChannelDto>>("ChannelList").Remove("AllList");
        }
    }
}