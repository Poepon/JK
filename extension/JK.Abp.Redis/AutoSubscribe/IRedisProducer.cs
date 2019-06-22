using System.Threading.Tasks;
using Abp.Dependency;

namespace JK.Abp.Redis.AutoSubscribe
{
    public interface IRedisProducer : ITransientDependency
    {
        Task PublishAsync<T>(string channel, T message);
    }
}
