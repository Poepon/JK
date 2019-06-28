using System;
using Abp.Runtime.Caching.Redis;
using StackExchange.Redis;

namespace JK.Abp.RedisCache.Sentinel
{
    public class AbpRedisCacheDatabaseProvider : IAbpRedisCacheDatabaseProvider
    {
        private readonly AbpRedisCacheOptions _options;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;
        private bool _isSubscribe = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="AbpRedisCacheDatabaseProvider"/> class.
        /// </summary>
        public AbpRedisCacheDatabaseProvider(AbpRedisCacheOptions options)
        {
            _options = options;
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.Value.GetDatabase(_options.DatabaseId);
        }

        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            var options = ConfigurationOptions.Parse(_options.ConnectionString);
            options.CommandMap = CommandMap.Sentinel;
            options.TieBreaker = "";
            var connection = ConnectionMultiplexer.Connect(options);
            if (_isSubscribe == false)
            {
                _isSubscribe = true;
                connection.GetSubscriber().Subscribe("+switch-master", (channel, message) =>
                {
                    Console.WriteLine((string)message);
                });
            }
            return connection;
        }
    }
}