using System;
using Microsoft.Extensions.DependencyInjection;
using Abp.Dependency;

namespace Volo.Abp.RabbitMQ
{
    public class RabbitMqMessageConsumerFactory : IRabbitMqMessageConsumerFactory, ISingletonDependency, IDisposable
    {
        protected IServiceScope ServiceScope { get; }

        public RabbitMqMessageConsumerFactory(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScope = serviceScopeFactory.CreateScope();
        }

        public IRabbitMqMessageConsumer Create(
            ExchangeDeclareConfiguration exchange,
            QueueDeclareConfiguration queue,
            QOSConfiguration consumerConfiguration,
            string connectionName = null)
        {
            var consumer = ServiceScope.ServiceProvider.GetRequiredService<RabbitMqMessageConsumer>();
            consumer.Initialize(exchange, queue, consumerConfiguration, connectionName);
            return consumer;
        }

        public void Dispose()
        {
            ServiceScope?.Dispose();
        }
    }
}