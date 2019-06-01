using Abp.Dependency;
using System.Threading.Tasks;

namespace Abp.RabbitMQ.AutoSubscribe
{
    public interface IRabbitMQProducer<T>
    {
        Task PublishAsync(T message);

        Task<bool> PublishAsync(T message, bool waitForConfirms);
    }
}
