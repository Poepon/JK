using Abp.Json;
using Abp.Runtime.Caching.Redis;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Abp.Redis.Events
{

    public abstract class BaseRedisConsumer<T> : IRedisConsumer<T>
    {
        private readonly IAbpRedisCacheDatabaseProvider _databaseProvider;
        private readonly IConnectionMultiplexer _conn;

        protected string ChannelName { get; }

        public BaseRedisConsumer(IAbpRedisCacheDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
            _conn = _databaseProvider.GetDatabase().Multiplexer;
            ChannelName = typeof(T).FullName;
        }
        public abstract Task ConsumeAsync(T message);

        public void Initialize()
        {
            var sub = _conn.GetSubscriber();
            sub.Subscribe(ChannelName, (c, v) =>
            {
                var message = v.ToString().FromJsonString<T>();
                ConsumeAsync(message);
            });
            Console.WriteLine($"订阅Redis事件.{this.GetType().FullName}");
        }

        public Task Unsubscribe()
        {
            var sub = _conn.GetSubscriber();
            return sub.UnsubscribeAsync(ChannelName);
        }
    }
}
