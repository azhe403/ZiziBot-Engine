using CacheTower;
using StackExchange.Redis;

namespace ZiziBot.Caching.Redis;

internal class RedisLayerProvider(
    string connectionString,
    RedisLayerOptions options
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

        await Database.KeyDeleteAsync(CacheKey(cacheKey));
    }

    public async ValueTask FlushAsync()
    {
        await Connect();

        var redisEndpoints = Connection.GetEndPoints();
        foreach (var endpoint in redisEndpoints)
        {
            var keys = Connection.GetServer(endpoint).KeysAsync(options.DatabaseIndex, pattern: $"{options.PrefixRoot}*");

            await foreach (var redisKey in keys)
            {
                await Database.KeyDeleteAsync(redisKey);
            }
        }
    }

    public async ValueTask<CacheEntry<T>?> GetAsync<T>(string cacheKey)
    {
        await Connect();

        var redisValue = await Database.StringGetAsync(CacheKey(cacheKey));
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
        await Database.StringSetAsync(CacheKey(cacheKey), redisValue, expiryOffset);
    }

    async Task Connect()
    {
        Connection = await ConnectionMultiplexer.ConnectAsync(ConnectionString);
        Database = Connection.GetDatabase(options.DatabaseIndex);
    }

    string CacheKey(string cacheKey)
    {
        return options.PrefixRoot.IsNullOrEmpty() ? $"{cacheKey}".Replace("/", ":") : $"{options.PrefixRoot}/{cacheKey}".Replace("/", ":");
    }
}