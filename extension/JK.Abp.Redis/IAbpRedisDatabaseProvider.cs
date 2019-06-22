using StackExchange.Redis;

namespace JK.Abp.Redis
{
    /// <summary>
    /// Used to get <see cref="IDatabase"/> for Redis cache.
    /// </summary>
    public interface IAbpRedisDatabaseProvider
    {
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        IDatabase GetDatabase();
    }
}
