using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Redis.Events
{
    public interface IRedisProducer<T> : ITransientDependency
    {
        Task PublishAsync(T message);
    }
}
