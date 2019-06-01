using Abp.Redis;
using System;
using System.Threading.Tasks;

namespace JK.Redis.Test
{
    public class Test1RedisConsumer : IRedisConsumer<string>
    {
        public Task ConsumeAsync(string message)
        {
            Console.WriteLine("run test1.");
            return Task.CompletedTask;
        }
        
        public string GetChannelName()
        {
            return "Test1";
        }

        public Type GetMessageType()
        {
            return typeof(string);
        }
    }
    public class Message
    {
        public string Text { get; set; }
    }
    public class Test2RedisConsumer : IRedisConsumer<Message>
    {
        public Task ConsumeAsync(Message message)
        {
            Console.WriteLine("run test2.");
            return Task.CompletedTask;
        }

        public string GetChannelName()
        {
            return "Test2";
        }

        public Type GetMessageType()
        {
            return typeof(Message);
        }
    }
}
