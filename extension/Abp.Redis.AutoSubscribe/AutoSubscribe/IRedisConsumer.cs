using System;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Redis
{   
    public interface IRedisConsumer
    {

    }
    public interface IRedisConsumer<T> : IRedisConsumer, ISingletonDependency
    {
        string GetChannelName();

        Type GetMessageType();

        Task ConsumeAsync(T message);
    }
}
