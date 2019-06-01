using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Redis
{
    public interface IRedisProducer<T> : ITransientDependency
    {
        Task PublishAsync(T message);
    }
}
