using Abp.Dependency;
using System.Threading.Tasks;

namespace Abp.RabbitMQ
{
    public interface IProducer<T>
    {
        Task PublishAsync(T message);

        Task<bool> PublishAsync(T message, bool waitForConfirms);
    }
}
