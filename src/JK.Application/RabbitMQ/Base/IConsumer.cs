using Abp.Dependency;
using System.Threading.Tasks;

namespace Abp.RabbitMQ
{
    public interface IConsumer<T>
    {
        void Initialize();

        Task ConsumeAsync(T message);
    }
}
