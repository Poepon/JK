using Abp.RabbitMQ.AutoSubscribe;
using System;
using System.Threading.Tasks;

namespace JK.Books.RabbitMQ
{

    public class QiDianChapterRabbitMQConsumer : RabbitMQConsumerBase<BookMessage>, IRabbitMQConsumer<BookMessage>
    {
        public override Task ConsumeAsync(BookMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
