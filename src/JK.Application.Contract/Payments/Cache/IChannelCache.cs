using System.Collections.Generic;
using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using JK.Payments.Bacis;
using JK.Payments.Dto;

namespace JK.Payments.Cache
{
    public interface IChannelCache : IEntityCache<ChannelDto>, ITransientDependency
    {
        ChannelDto Get(string code);

        IReadOnlyList<ChannelDto> GetActiveList();

        IReadOnlyList<ChannelDto> GetAll();
    }
}