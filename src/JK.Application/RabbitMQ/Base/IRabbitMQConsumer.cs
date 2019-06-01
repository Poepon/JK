using Abp.Dependency;
using System.Threading.Tasks;

namespace Abp.RabbitMQ
{
    public interface IRabbitMQConsumer<T>
    {
        void Initialize();

        Task ConsumeAsync(T message);
    }
}
