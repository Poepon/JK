using System;
using System.Linq;
using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Runtime.Caching;
using JK.Payments.Dto;
using JK.Payments.Enumerates;
using JK.Payments.Integration;
using JK.Payments.Integration.Dto;

namespace JK.Payments.Cache
{
    public class ResultCodeCache : EntityCache<ResultCode, ResultCodeDto>, IResultCodeCache
    {
        public ResultCodeCache(ICacheManager cacheManager, IRepository<ResultCode, int> repository, string cacheName = null) : base(cacheManager, repository, cacheName)
        {
        }

        public ResultCodeMean GetCodeMean(int companyId, string code)
        {
            var value = CacheManager.GetCache<string, ResultCodeMean>("ResultCodeMean").Get($"cid:{companyId}-code:{code}", () =>
             {
                 var entity = Repository.GetAll().FirstOrDefault(c => c.CompanyId == companyId && c.Code == code);
                 if (entity == null)
                     return ResultCodeMean.Unknown;
                 return entity.Mean;
             });
            return value;
        }

        public override void HandleEvent(EntityChangedEventData<ResultCode> eventData)
        {
            base.HandleEvent(eventData);

        }
    }
}