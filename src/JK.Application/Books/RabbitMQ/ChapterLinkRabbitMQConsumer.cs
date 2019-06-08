using Abp.RabbitMQ.AutoSubscribe;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JK.Books.RabbitMQ
{
    public class ChapterLinkRabbitMQConsumer : RabbitMQConsumerBase<BookMessage>, IRabbitMQConsumer<BookMessage>
    {
        public override Task ConsumeAsync(BookMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
