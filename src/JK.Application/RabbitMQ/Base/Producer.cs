using RabbitMQ.Client;
using System.Threading.Tasks;
using Volo.Abp.RabbitMQ;

namespace Abp.RabbitMQ
{
    public abstract class Producer<T> : IProducer<T>
    {
        public Producer(IConnectionPool connectionPool, IRabbitMqSerializer serializer)
        {
            ConnectionPool = connectionPool;
            Serializer = serializer;
            var type = typeof(T);
            ExchangeName = type.FullName;
        }
        protected IConnectionPool ConnectionPool { get; }
        protected IRabbitMqSerializer Serializer { get; }
        protected abstract string ConnectionName { get; }
        protected string ExchangeName { get; }
        protected virtual string ExchangeType { get; } = "direct";

        public async Task PublishAsync(T message)
        {
            await PublishAsync(message, false);
        }

        public Task<bool> PublishAsync(T message, bool waitForConfirms)
        {
            bool result = !waitForConfirms;
            var eventName = (typeof(T).FullName);
            var body = Serializer.Serialize(message);

            using (var channel = ConnectionPool.Get(ConnectionName).CreateModel())
            {
                channel.ExchangeDeclare(
                    ExchangeName,
                    ExchangeType,
                    durable: true
                );

                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = RabbitMqConsts.DeliveryModes.Persistent;

                channel.BasicPublish(
                   exchange: ExchangeName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                );
                if (waitForConfirms)
                    result = channel.WaitForConfirms();
            }

            return Task.FromResult(result);
        }

    }
}
