using Abp.Dependency;
using Abp.Reflection;
using JK.Serialization;
using System;
using System.Threading.Tasks;
using Volo.Abp.RabbitMQ;
using System.Collections.Generic;

namespace Abp.RabbitMQ.AutoSubscribe
{
    public class AutoRabbitMQConsumer : ISingletonDependency
    {
        private readonly ITypeFinder typeFinder;
        private readonly IIocResolver iocResolver;
        private readonly IObjectSerializer objectSerializer;
        private readonly IRabbitMqMessageConsumerFactory consumerFactory;
        private readonly Dictionary<Type, IRabbitMqMessageConsumer> Consumers = new Dictionary<Type, IRabbitMqMessageConsumer>();
        public AutoRabbitMQConsumer(
            ITypeFinder typeFinder,
            IIocResolver iocResolver,
            IObjectSerializer objectSerializer,
            IRabbitMqMessageConsumerFactory consumerFactory)
        {
            this.typeFinder = typeFinder;
            this.iocResolver = iocResolver;
            this.objectSerializer = objectSerializer;
            this.consumerFactory = consumerFactory;
        }

        public void Subscribe()
        {
            var consumers = typeFinder.Find(t => t.IsClass && typeof(IRabbitMQConsumer).IsAssignableFrom(t));
            foreach (var item in consumers)
            {
                var obj = iocResolver.Resolve(item);
                Type messageType = (Type)item.InvokeMember("GetMessageType", System.Reflection.BindingFlags.InvokeMethod, null, obj, null);
                ExchangeDeclareConfiguration exchange = (ExchangeDeclareConfiguration)item.InvokeMember("GetExchangeDeclare", System.Reflection.BindingFlags.InvokeMethod, null, obj, null);
                QueueDeclareConfiguration queue = (QueueDeclareConfiguration)item.InvokeMember("GetQueueDeclare", System.Reflection.BindingFlags.InvokeMethod, null, obj, null);
                QOSConfiguration qOS = (QOSConfiguration)item.InvokeMember("GetQOSConfiguration", System.Reflection.BindingFlags.InvokeMethod, null, obj, null);
                string ConnectionName = (string)item.InvokeMember("GetConnectionName", System.Reflection.BindingFlags.InvokeMethod, null, obj, null);
                var consumer = consumerFactory.Create(
                    exchange,
                           //new ExchangeDeclareConfiguration(
                           //    ExchangeName,
                           //    ExchangeType,
                           //    durable: Durable
                           //),
                           queue,
                           //new QueueDeclareConfiguration(
                           //    QueueName,
                           //    durable: Durable,
                           //    exclusive: Exclusive,
                           //    autoDelete: AutoDelete
                           //),
                           qOS,
                           ConnectionName
                       );
                consumer.OnMessageReceived((model, ea) =>
                {
                    var eventName = ea.RoutingKey;

                    var eventData = objectSerializer.Deserialize(messageType, ea.Body);
                    var task = (Task)item.InvokeMember("ConsumeAsync", System.Reflection.BindingFlags.InvokeMethod, null, obj, new[] { eventData });
                    return task;
                });
                string routingKey = (string)item.InvokeMember("GetRoutingKey", System.Reflection.BindingFlags.InvokeMethod, null, obj, null);
                consumer.BindAsync(routingKey);
                Consumers.Add(item, consumer);
                Console.WriteLine("1");
            }
        }
    }
}
