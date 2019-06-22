using System;
using System.Threading.Tasks;
using Abp.Dependency;

namespace JK.Abp.RabbitMQ.AutoSubscribe
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

    public abstract class RabbitMQConsumerBase<T> : IRabbitMQConsumer<T>
    {
        public abstract Task ConsumeAsync(T message);

        public virtual string GetConnectionName()
        {
            return null;
        }

        public virtual ExchangeDeclareConfiguration GetExchangeDeclare()
        {
            return new ExchangeDeclareConfiguration(typeof(T).FullName, ExchangeType.direct, true, false);
        }

        public virtual Type GetMessageType()
        {
            return typeof(T);
        }

        public virtual QOSConfiguration GetQOSConfiguration()
        {
            return new QOSConfiguration(0, false);
        }

        public virtual QueueDeclareConfiguration GetQueueDeclare()
        {
            return new QueueDeclareConfiguration(this.GetType().FullName, true, false, false);
        }

        public virtual string GetRoutingKey()
        {
            return "#";
        }
    }
}
