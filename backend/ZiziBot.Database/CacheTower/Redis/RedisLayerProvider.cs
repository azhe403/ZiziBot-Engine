using CacheTower;
using StackExchange.Redis;
using ZiziBot.Common.Utils;

namespace ZiziBot.Database.CacheTower.Redis;

internal class RedisLayerProvider(
    string connectionString,
    RedisLayerOptions options
)
    : IDistributedCacheLayer
{
    private string ConnectionString { get; set; } = connectionString;
    private ConnectionMultiplexer Connection { get; set; }
    private IDatabaseAsync Database { get; set; }
    private static string PrefixRoot => ValueConst.UniqueKey;

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
            var keys = Connection.GetServer(endpoint).KeysAsync(options.DatabaseIndex, pattern: $"{PrefixRoot}*");

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

        return null;
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

    private async Task Connect()
    {
        Connection = await ConnectionMultiplexer.ConnectAsync(ConnectionString);
        Database = Connection.GetDatabase(options.DatabaseIndex);
    }

    private string CacheKey(string cacheKey)
    {
        return PrefixRoot.IsNullOrEmpty() ? $"{cacheKey}".Replace("/", ":") : $"{PrefixRoot}/{cacheKey}".Replace("/", ":");
    }
}