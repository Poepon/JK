using System.Threading.Tasks;
using JK.Abp.Serialization;
using RabbitMQ.Client;

namespace JK.Abp.RabbitMQ.AutoSubscribe
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        public RabbitMQProducer(IConnectionPool connectionPool, IObjectSerializer serializer)
        {
            ConnectionPool = connectionPool;
            Serializer = serializer;
        }
        protected IConnectionPool ConnectionPool { get; }
        protected IObjectSerializer Serializer { get; }
        protected string ConnectionName { get; }

        public async Task PublishAsync<T>(ExchangeDeclareConfiguration exchange, string routingKey, T message)
        {
            await PublishAsync(exchange, routingKey, message, false);
        }

        public Task<bool> PublishAsync<T>(ExchangeDeclareConfiguration exchange, string routingKey, T message, bool waitForConfirms)
        {
            bool result = !waitForConfirms;
            var body = Serializer.Serialize(message);

            using (var channel = ConnectionPool.Get(ConnectionName).CreateModel())
            {
                channel.ExchangeDeclare(
                    exchange.ExchangeName,
                    exchange.Type,
                    durable: exchange.Durable
                );

                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = RabbitMqConsts.DeliveryModes.Persistent;

                channel.BasicPublish(
                   exchange: exchange.ExchangeName,
                    routingKey: routingKey,
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
