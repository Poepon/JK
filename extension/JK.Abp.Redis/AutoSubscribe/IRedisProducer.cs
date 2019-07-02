using System.Threading.Tasks;
using Abp.Dependency;

namespace JK.Abp.RedisCache.Events.AutoSubscribe
{
    public interface IRedisProducer : ITransientDependency
    {
        Task PublishAsync<T>(string channel, T message);
    }
}
