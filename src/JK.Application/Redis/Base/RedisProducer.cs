using Abp.Json;
using Abp.Runtime.Caching.Redis;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Abp.Redis.Events
{
    public class RedisProducer<T> : IRedisProducer<T>
    {
        private readonly IAbpRedisCacheDatabaseProvider _databaseProvider;
        private readonly IConnectionMultiplexer _conn;

        protected string ChannelName { get; }

        public RedisProducer(IAbpRedisCacheDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
            _conn = _databaseProvider.GetDatabase().Multiplexer;
            ChannelName = typeof(T).FullName;
        }

        public async Task PublishAsync(T message)
        {
            var subscriber = _conn.GetSubscriber();
            await subscriber.PublishAsync(ChannelName, message.ToJsonString());
        }
    }
}
