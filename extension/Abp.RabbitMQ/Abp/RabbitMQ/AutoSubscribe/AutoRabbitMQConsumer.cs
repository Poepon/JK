using Abp.Dependency;
using Abp.Reflection;
using System.Threading.Tasks;
using Volo.Abp.RabbitMQ;

namespace Abp.RabbitMQ.AutoSubscribe
{
    public class AutoRabbitMQConsumer : ISingletonDependency
    {
        private readonly ITypeFinder typeFinder;
        private readonly IIocResolver iocResolver;
        private readonly IRabbitMqMessageConsumerFactory consumerFactory;

        public AutoRabbitMQConsumer(
            ITypeFinder typeFinder,
            IIocResolver iocResolver,
            IRabbitMqMessageConsumerFactory consumerFactory)
        {
            this.typeFinder = typeFinder;
            this.iocResolver = iocResolver;
            this.consumerFactory = consumerFactory;
        }

        public void Init()
        {
            var consumers = typeFinder.Find(t => t.IsClass && typeof(IRabbitMQConsumer).IsAssignableFrom(t));
            foreach (var item in consumers)
            {
                var obj = iocResolver.Resolve(item);
                var consumer = consumerFactory.Create(
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
                consumer.OnMessageReceived((model, ea) =>
                {
                    var eventName = ea.RoutingKey;

                    var eventData = Serializer.Deserialize(typeof(T), ea.Body);

                    await ConsumeAsync((T)eventData);

                    return Task.CompletedTask;
                });
            }

        }
    }
}
