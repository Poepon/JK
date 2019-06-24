using System.Collections.Generic;
using Abp.Dependency;

namespace JK.Payments.Cache
{
    public interface IChannelCache : ITransientDependency
    {
        ChannelDto Get(int id);

        IReadOnlyList<ChannelDto> GetActiveList();

        ChannelDto GetOrNull(int id);

        IReadOnlyList<ChannelDto> GetAll();
    }
}