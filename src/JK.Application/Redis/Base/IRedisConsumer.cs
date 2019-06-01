using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Redis
{
    public interface IRedisConsumer<T> : ISingletonDependency
    {
        Task ConsumeAsync(T message);

        void Initialize();

        Task Unsubscribe();
    }
}
