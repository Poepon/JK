using System.Threading.Tasks;
using Abp.Runtime.Caching.Redis;
using JK.Abp.Serialization;

namespace JK.Abp.RedisCache.Events.AutoSubscribe
{
    public class RedisProducer : IRedisProducer
    {
        private readonly IAbpRedisCacheDatabaseProvider databaseProvider;
        private readonly IObjectSerializer objectSerializer;

        public RedisProducer(IAbpRedisCacheDatabaseProvider databaseProvider,
            IObjectSerializer objectSerializer)
        {
            this.objectSerializer = objectSerializer;
            this.databaseProvider = databaseProvider;
        }

        public async Task PublishAsync<T>(string channel, T message)
        {
            var subscriber = databaseProvider.GetDatabase().Multiplexer.GetSubscriber();
            await subscriber.PublishAsync(channel, objectSerializer.Serialize(message));
        }
    }
}
