using JK.Abp.Serialization;
using System.Threading.Tasks;

namespace JK.Abp.Redis.AutoSubscribe
{
    public class RedisProducer : IRedisProducer
    {
        private readonly IAbpRedisDatabaseProvider databaseProvider;
        private readonly IObjectSerializer objectSerializer;

        public RedisProducer(IAbpRedisDatabaseProvider databaseProvider,
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
