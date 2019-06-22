using System;
using Abp.Dependency;
using StackExchange.Redis;

namespace JK.Abp.Redis
{
    /// <summary>
    /// Implements <see cref="IAbpRedisDatabaseProvider"/>.
    /// </summary>
    public class AbpRedisDatabaseProvider : IAbpRedisDatabaseProvider, ISingletonDependency
    {
        private readonly AbpRedisOptions _options;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpRedisCacheDatabaseProvider"/> class.
        /// </summary>
        public AbpRedisDatabaseProvider(AbpRedisOptions options)
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
            return ConnectionMultiplexer.Connect(_options.ConnectionString);
        }
    }
}
