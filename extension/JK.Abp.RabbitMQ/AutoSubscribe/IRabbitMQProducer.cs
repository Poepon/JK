using System.Threading.Tasks;
using Abp.Dependency;

namespace JK.Abp.RabbitMQ.AutoSubscribe
{
    public interface IRabbitMQProducer : ITransientDependency
    {
        Task PublishAsync<T>(ExchangeDeclareConfiguration exchange, string routingKey, T message);

        Task<bool> PublishAsync<T>(ExchangeDeclareConfiguration exchange, string routingKey, T message, bool waitForConfirms);
    }
}
