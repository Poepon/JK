using System;
using System.Threading.Tasks;
using Abp.Dependency;

namespace JK.Abp.RedisCache.Events.AutoSubscribe
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
