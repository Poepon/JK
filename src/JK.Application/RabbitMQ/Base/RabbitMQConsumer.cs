using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using Volo.Abp.RabbitMQ;

namespace Abp.RabbitMQ
{
    public abstract class RabbitMQConsumer<T> : IRabbitMQConsumer<T>
    {
        protected IRabbitMqSerializer Serializer { get; }

        protected IRabbitMqMessageConsumerFactory MessageConsumerFactory { get; }
        protected IRabbitMqMessageConsumer Consumer { get; private set; }

        protected string ExchangeName { get; }

        /// <summary>
        /// direct fanout topic headers
        /// </summary>
        protected virtual string ExchangeType { get; } = "direct";

        protected virtual bool Exclusive { get; } = false;
        protected virtual bool Durable { get; } = true;
        protected virtual bool AutoDelete { get; } = false;

        protected virtual ConsumerConfiguration Configuration => new ConsumerConfiguration(0, false);

        protected string QueueName { get; }
        protected abstract string ConnectionName { get; }

        public RabbitMQConsumer(
          IRabbitMqSerializer serializer,
          IRabbitMqMessageConsumerFactory messageConsumerFactory)
        {
            Serializer = serializer;
            MessageConsumerFactory = messageConsumerFactory;
            var type = typeof(T);
            ExchangeName = type.FullName;
            QueueName = this.GetType().FullName;
        }
        public void Initialize()
        {
            Consumer = MessageConsumerFactory.Create(
                new ExchangeDeclareConfiguration(
                    ExchangeName,
                    ExchangeType,
                    durable: Durable
                ),
                new QueueDeclareConfiguration(
                    QueueName,
                    durable: Durable,
                    exclusive: Exclusive,
                    autoDelete: AutoDelete
                ),
                Configuration,
                ConnectionName
            );

            Consumer.OnMessageReceived(ProcessEventAsync);
            Consumer.BindAsync(typeof(T).FullName);
            System.Console.WriteLine($"Initialize {this.GetType().FullName}.");
        }

        private async Task ProcessEventAsync(IModel channel, BasicDeliverEventArgs ea)
        {
            var eventName = ea.RoutingKey;

            var eventData = Serializer.Deserialize(ea.Body, typeof(T));

            await ConsumeAsync((T)eventData);
        }

        public abstract Task ConsumeAsync(T message);

    }
}
