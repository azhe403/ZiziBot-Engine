using CacheTower;
using MongoDB.Driver;
using ZiziBot.DataSource.Utils;

namespace ZiziBot.Caching.MongoDb;

internal class MongoLayerProvider(MongoLayerOptions options) : IDistributedCacheLayer
{
    private string DbName => options.ConnectionString.ToMongoUrl().DatabaseName;
    private MongoClient MongoClient => new MongoClient(options.ConnectionString);
    private IMongoDatabase MongoDatabase => MongoClient.GetDatabase(DbName);
    private IMongoCollection<MongoCacheEntry> Collection => MongoDatabase.GetCollection<MongoCacheEntry>("_cache");

    public async ValueTask FlushAsync()
    {
        await Collection.DeleteManyAsync(x => true);
    }

    public async ValueTask CleanupAsync()
    {
        await Collection.DeleteManyAsync(x => x.Expiry < DateTime.UtcNow);
    }

    public async ValueTask EvictAsync(string cacheKey)
    {
        await Collection.DeleteOneAsync(x => x.CacheKey == cacheKey);
    }

    public async ValueTask<CacheEntry<T>?> GetAsync<T>(string cacheKey)
    {
        var cache = await Collection.Find(x => x.CacheKey == cacheKey).FirstOrDefaultAsync();
        var cacheEntry = default(CacheEntry<T>);

        if (cache != default)
            cacheEntry = new((T)cache.Value, cache.Expiry);

        return cacheEntry;
    }

    public async ValueTask SetAsync<T>(string cacheKey, CacheEntry<T> cacheEntry)
    {
        await Collection.FindOneAndReplaceAsync(x => x.CacheKey == cacheKey, new MongoCacheEntry {
            CacheKey = cacheKey,
            CreatedDate = DateTime.UtcNow,
            Value = cacheEntry.Value.ToJson(),
            Expiry = cacheEntry.Expiry
        }, new() {
            IsUpsert = true
        });
    }

    public async ValueTask<bool> IsAvailableAsync(string cacheKey)
    {
        return true;
    }
}