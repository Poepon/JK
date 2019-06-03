using Abp.Dependency;
using System.Threading.Tasks;
using Volo.Abp.RabbitMQ;

namespace Abp.RabbitMQ.AutoSubscribe
{
    public interface IRabbitMQProducer : ITransientDependency
    {
        Task PublishAsync<T>(ExchangeDeclareConfiguration exchange, string routingKey, T message);

        Task<bool> PublishAsync<T>(ExchangeDeclareConfiguration exchange, string routingKey, T message, bool waitForConfirms);
    }
}
