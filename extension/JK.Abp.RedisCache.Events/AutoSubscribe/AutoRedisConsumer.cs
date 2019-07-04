using System;
using System.Reflection;
using Abp.Dependency;
using Abp.Reflection;
using Abp.Runtime.Caching.Redis;
using JK.Abp.Serialization;

namespace JK.Abp.RedisCache.Events.AutoSubscribe
{
    public class AutoRedisConsumer : ISingletonDependency
    {
        private readonly ITypeFinder typeFinder;
        private readonly IIocResolver iocResolver;
        private readonly IObjectSerializer objectSerializer;
        private readonly IAbpRedisCacheDatabaseProvider databaseProvider;

        public AutoRedisConsumer(
            ITypeFinder typeFinder,
            IIocResolver iocResolver,
            IObjectSerializer objectSerializer,
            IAbpRedisCacheDatabaseProvider databaseProvider)
        {
            this.typeFinder = typeFinder;
            this.iocResolver = iocResolver;
            this.objectSerializer = objectSerializer;
            this.databaseProvider = databaseProvider;
        }

        public void Subscribe()
        {
            var consumers = typeFinder.Find(t => t.IsClass && typeof(IRedisConsumer).IsAssignableFrom(t));
            var sub = databaseProvider.GetDatabase().Multiplexer.GetSubscriber();
            foreach (var item in consumers)
            {
                var obj = iocResolver.Resolve(item);
                var channel = (string)item.InvokeMember("GetChannelName", BindingFlags.InvokeMethod, null, obj, null);
                sub.Subscribe(channel, (c, v) =>
                {
                    var messageType = (Type)item.InvokeMember("GetMessageType", BindingFlags.InvokeMethod, null, obj, null);
                    var message = objectSerializer.Deserialize(messageType, v);
                    item.InvokeMember("ConsumeAsync", BindingFlags.InvokeMethod, null, obj, new[] { message });
                });
            }
        }

        public void Unsubscribe()
        {
            var consumers = typeFinder.Find(t => t.IsClass && typeof(IRedisConsumer).IsAssignableFrom(t));
            var sub = databaseProvider.GetDatabase().Multiplexer.GetSubscriber();
            foreach (var item in consumers)
            {
                var obj = iocResolver.Resolve(item);
                var channel = (string)item.InvokeMember("GetChannelName", BindingFlags.InvokeMethod, null, obj, null);
                sub.Unsubscribe(channel);
            }
        }
    }
}
