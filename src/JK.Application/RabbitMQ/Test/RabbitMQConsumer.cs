using Abp.RabbitMQ.AutoSubscribe;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.RabbitMQ;

namespace JK.RabbitMQ.Test
{
    public class RabbitMQConsumer : IRabbitMQConsumer<string>
    {
        public Task ConsumeAsync(string message)
        {
            Console.WriteLine("This is RabbitMQ.");
            return Task.CompletedTask;
        }

        public ExchangeDeclareConfiguration GetExchangeDeclare()
        {
            return new ExchangeDeclareConfiguration(
                         typeof(string).FullName,
                         "direct",
                         durable: false
                     );
        }

        public QueueDeclareConfiguration GetQueueDeclare()
        {
            return new QueueDeclareConfiguration(
                        this.GetType().FullName,
                        durable: false,
                        exclusive: false,
                        autoDelete: true
                    );
        }

        public QOSConfiguration GetQOSConfiguration()
        {
            return new QOSConfiguration(0, false);
        }

        public string GetConnectionName()
        {
            return null;
        }

        public Type GetMessageType()
        {
            return typeof(string);
        }

        public string GetRoutingKey()
        {
            return "#";
        }
    }
}
