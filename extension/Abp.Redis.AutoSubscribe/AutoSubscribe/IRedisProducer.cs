using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Redis
{
    public interface IRedisProducer : ITransientDependency
    {
        Task PublishAsync<T>(string channel, T message);
    }
}
