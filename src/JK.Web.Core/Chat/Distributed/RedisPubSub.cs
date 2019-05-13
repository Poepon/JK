using Abp.Dependency;
using Abp.Runtime.Caching.Redis;
using StackExchange.Redis;
using System.Threading.Tasks;
using JK.Chat.Dto;
using JK.Chat.WebSocketPackage;

namespace JK.Chat.Distributed
{
    public class RedisPubSub : ISingletonDependency
    {
        private readonly IAbpRedisCacheDatabaseProvider _databaseProvider;
        private readonly IDistributedHandler _distributedHandler;

        public RedisPubSub(IAbpRedisCacheDatabaseProvider databaseProvider,
            IDistributedHandler distributedHandler)
        {
            _databaseProvider = databaseProvider;
            _distributedHandler = distributedHandler;
        }

        protected IDatabase Database
        {
            get { return _databaseProvider.GetDatabase(); }
        }
        public void Subscribe(string channel)
        {
            var _connection = Database.Multiplexer;
            _connection.GetSubscriber().Subscribe(channel, (s, r) =>
              {
                  _distributedHandler.HandleEventAsync(r);
              });
        }
        public Task SubscribeAsync(string channel)
        {
            var _connection = Database.Multiplexer;
            return _connection.GetSubscriber().SubscribeAsync(channel, (s, r) =>
             {
                 _distributedHandler.HandleEventAsync(r);
             });
        }
        public long Publish<T>(string channel, T msg)
        {
            ISubscriber sub = Database.Multiplexer.GetSubscriber();
            return sub.Publish(channel, msg.SerializeToBytes(MessageDataType.MessagePack));
        }
        public async Task<long> PublishAsync<T>(string channel, T msg)
        {
            ISubscriber sub = Database.Multiplexer.GetSubscriber();
            return await sub.PublishAsync(channel, msg.SerializeToBytes(MessageDataType.MessagePack));
        }

        public async Task Unsubscribe(string server)
        {
            var _connection = Database.Multiplexer;
            await _connection.GetSubscriber().UnsubscribeAsync(server);
        }
        public async Task UnsubscribeAll()
        {
            var _connection = Database.Multiplexer;
            await _connection.GetSubscriber().UnsubscribeAllAsync();
        }

    }
}
