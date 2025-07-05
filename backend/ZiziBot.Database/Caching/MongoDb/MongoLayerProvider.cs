using CacheTower;
using MongoDB.Driver;
using Serilog;
using ZiziBot.Database.Utils;
using ObjectId = MongoDB.Bson.ObjectId;

namespace ZiziBot.Database.Caching.MongoDb;

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

        if (cache != null)
            cacheEntry = new(cache.Value.ToObject<T>(), cache.Expiry);

        return cacheEntry;
    }

    public async ValueTask SetAsync<T>(string cacheKey, CacheEntry<T> cacheEntry)
    {
        var existing = await Collection.Find(x => x.CacheKey == cacheKey).FirstOrDefaultAsync();
        var entry = new MongoCacheEntry {
            Id = existing?.Id ?? ObjectId.GenerateNewId(), // Keep existing ID or generate new one
            CacheKey = cacheKey,
            CreatedDate = existing?.CreatedDate ?? DateTime.UtcNow, // Keep original creation date
            Value = cacheEntry.Value.ToJson(),
            Expiry = cacheEntry.Expiry
        };

        await Collection.ReplaceOneAsync(x => x.CacheKey == cacheKey, entry, new ReplaceOptions() {
            IsUpsert = true
        });
    }

    public async ValueTask<bool> IsAvailableAsync(string cacheKey)
    {
        var documentCount = await Collection.EstimatedDocumentCountAsync();

        var isAvailable = documentCount >= 0;
        Log.Verbose("IsAvailable CacheTower mongo layer. Key: {CacheKey}. Result: {IsAvailable}", cacheKey, isAvailable);

        return isAvailable;
    }
}