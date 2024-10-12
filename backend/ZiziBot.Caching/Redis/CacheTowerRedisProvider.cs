using CacheTower;
using CacheTower.Providers.Redis;
using StackExchange.Redis;

namespace ZiziBot.Caching.Redis;

public class CacheTowerRedisProvider(
    string connectionString,
    RedisCacheLayerOptions options
)
    : IDistributedCacheLayer
{
    string ConnectionString { get; set; } = connectionString;
    ConnectionMultiplexer Connection { get; set; }
    IDatabaseAsync Database { get; set; }

    public ValueTask CleanupAsync()
    {
        //Noop as Redis handles this directly
        return new ValueTask();
    }

    public async ValueTask EvictAsync(string cacheKey)
    {
        await Connect();

        await Database.KeyDeleteAsync(cacheKey);
    }

    public async ValueTask FlushAsync()
    {
        await Connect();

        var redisEndpoints = Connection.GetEndPoints();
        foreach (var endpoint in redisEndpoints)
        {
            await Connection.GetServer(endpoint).FlushDatabaseAsync(options.DatabaseIndex);
        }
    }

    public async ValueTask<CacheEntry<T>?> GetAsync<T>(string cacheKey)
    {
        await Connect();

        var redisValue = await Database.StringGetAsync(cacheKey);
        if (redisValue != RedisValue.Null)
        {
            using var stream = new MemoryStream(redisValue);
            return options.Serializer.Deserialize<CacheEntry<T>>(stream);
        }

        return default;
    }

    public async ValueTask<bool> IsAvailableAsync(string cacheKey)
    {
        await Connect();

        return await new ValueTask<bool>(Connection.IsConnected);
    }

    public async ValueTask SetAsync<T>(string cacheKey, CacheEntry<T> cacheEntry)
    {
        await Connect();

        var expiryOffset = cacheEntry.Expiry - DateTime.UtcNow;
        if (expiryOffset < TimeSpan.Zero)
        {
            return;
        }

        using var stream = new MemoryStream();
        options.Serializer.Serialize(stream, cacheEntry);
        stream.Seek(0, SeekOrigin.Begin);
        var redisValue = RedisValue.CreateFrom(stream);
        await Database.StringSetAsync(cacheKey, redisValue, expiryOffset);
    }

    async Task Connect()
    {
        Connection = await ConnectionMultiplexer.ConnectAsync(ConnectionString);
        Database = Connection.GetDatabase(options.DatabaseIndex);
    }
}