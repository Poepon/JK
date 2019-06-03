using Abp.Dependency;
using System;
using System.Threading.Tasks;
using Volo.Abp.RabbitMQ;

namespace Abp.RabbitMQ.AutoSubscribe
{
    public interface IRabbitMQConsumer
    {

    }
    public interface IRabbitMQConsumer<T> : IRabbitMQConsumer, ISingletonDependency
    {
        ExchangeDeclareConfiguration GetExchangeDeclare();

        QueueDeclareConfiguration GetQueueDeclare();

        QOSConfiguration GetQOSConfiguration();

        string GetConnectionName();

        string GetRoutingKey();

        Type GetMessageType();

        Task ConsumeAsync(T message);
    }
}
