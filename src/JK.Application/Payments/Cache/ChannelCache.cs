using System.Collections.Generic;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using JK.Payments.Bacis;

namespace JK.Payments.Cache
{
    public class ChannelCache : IChannelCache
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Channel> _channelRepository;

        public ChannelCache(ICacheManager cacheManager,IRepository<Channel> channelRepository)
        {
            _cacheManager = cacheManager;
            _channelRepository = channelRepository;
        }
        public ChannelDto Get(int id)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<ChannelDto> GetActiveList()
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<ChannelDto> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public ChannelDto GetOrNull(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}