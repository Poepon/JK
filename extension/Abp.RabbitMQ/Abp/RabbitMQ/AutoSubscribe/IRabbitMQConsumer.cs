using Abp.Dependency;
using System.Threading.Tasks;
using Volo.Abp.RabbitMQ;

namespace Abp.RabbitMQ.AutoSubscribe
{
    public interface IRabbitMQConsumer
    {

    }
    public interface IRabbitMQConsumer<T> : IRabbitMQConsumer
    {
        void Initialize();

        ExchangeDeclareConfiguration GetExchangeDeclare();

        QueueDeclareConfiguration GetQueueDeclare();

        QOSConfiguration GetQOSConfiguration();

        Task ConsumeAsync(T message);
    }
}
