using System.Collections.Generic;
using System.IO;
using System.Linq;
using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Runtime.Caching;
using JK.Payments.Enumerates;
using JK.Payments.Integration;
using JK.Payments.Integration.Dto.ApiParameters;

namespace JK.Payments.Cache
{
    public class ApiParameterCache : EntityCache<ApiParameter, ApiParameterDto>, IApiParameterCache
    {
        public ApiParameterCache(ICacheManager cacheManager, IRepository<ApiParameter, int> repository, string cacheName = null) : base(cacheManager, repository, cacheName)
        {
        }

        public IReadOnlyList<ApiParameterDto> GetApiParameters(int companyId, int channelId, ApiMethod method, ParameterType parameterType)
        {
            return CacheManager.GetCache("ApiParameters").Get($"{companyId}_{channelId}_{method}_{parameterType}", () =>
             {
                 var entities = Repository.GetAll().Where(p => p.CompanyId == companyId &&
                                                               p.ApiMethod == method &&
                                                               p.ParameterType == parameterType &&
                                                               p.SupportedChannels.Any(c => c.ChannelId == channelId))
                     .ToList();
                 return ObjectMapper.Map<List<ApiParameterDto>>(entities);
             });
        }

        public override void HandleEvent(EntityChangedEventData<ApiParameter> eventData)
        {
            base.HandleEvent(eventData);
            CacheManager.GetCache("ApiParameters");
        }
    }
}