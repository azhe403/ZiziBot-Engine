using CacheTower;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ZiziBot.Database.Sqlite;
using ZiziBot.Database.Sqlite.Entities;

namespace ZiziBot.Database.Caching.Sqlite;

internal class SqliteLayerProvider : ICacheLayer
{
    private readonly string _dbPath;
    private SqliteCachingDbContext Db => new SqliteCachingDbContext(_dbPath);

    public SqliteLayerProvider(string dbPath)
    {
        _dbPath = dbPath;

        Db.Database.EnsureCreated();
        Log.Verbose("Table structure for CacheTower sqlite created");
    }

    public async ValueTask FlushAsync()
    {
        Log.Verbose("Flush CacheTower sqlite layer");
        await Db.SqliteCache.ExecuteDeleteAsync();
        await Db.SaveChangesAsync();
        Log.Verbose("Flush CacheTower sqlite layer has done");
    }

    public async ValueTask CleanupAsync()
    {
        Log.Verbose("Cleanup CacheTower sqlite layer");
        await Db.SqliteCache.Where(x => x.ExpiryDate < DateTime.UtcNow).ExecuteDeleteAsync();
        await Db.SaveChangesAsync();
        Log.Verbose("Cleanup CacheTower sqlite layer done");
    }

    public async ValueTask EvictAsync(string cacheKey)
    {
        Log.Verbose("Evict CacheTower sqlite layer. Key: {CacheKey}", cacheKey);
        await Db.SqliteCache.Where(x => x.CacheKey == cacheKey).ExecuteDeleteAsync();
        await Db.SaveChangesAsync();
        Log.Verbose("Evict CacheTower sqlite layer has done. Key: {CacheKey}", cacheKey);
    }

    public async ValueTask<CacheEntry<T>?> GetAsync<T>(string cacheKey)
    {
        var cacheEntry = default(CacheEntry<T>);

        var obj = await Db.SqliteCache.AsNoTracking()
            .Where(x => x.CacheKey == cacheKey)
            .FirstOrDefaultAsync();

        if (obj != null)
        {
            var value = obj.Value.ToObject<T>();
            return new CacheEntry<T>(value, obj.ExpiryDate);
        }

        return cacheEntry;
    }

    public async ValueTask SetAsync<T>(string cacheKey, CacheEntry<T> cacheEntry)
    {
        var findCache = await Db.SqliteCache
            .Where(x => x.CacheKey == cacheKey)
            .FirstOrDefaultAsync();

        if (findCache == null)
        {
            Log.Verbose("Set CacheTower sqlite layer. Key: {CacheKey}", cacheKey);
            await Db.SqliteCache.AddAsync(new SqliteCacheEntity() {
                CacheKey = cacheKey,
                ExpiryDate = cacheEntry.Expiry,
                CreatedDate = DateTime.UtcNow,
                Value = cacheEntry.Value.ToJson(),
            });
        }
        else
        {
            Log.Verbose("Update CacheTower sqlite layer. Key: {CacheKey}", cacheKey);
            findCache.ExpiryDate = cacheEntry.Expiry;
            findCache.Value = cacheEntry.Value.ToJson();
        }

        await Db.SaveChangesAsync();
        Log.Verbose("Save CacheTower sqlite layer done");
    }

    public async ValueTask<bool> IsAvailableAsync(string cacheKey)
    {
        var obj = await Db.SqliteCache.AsNoTracking()
            .Where(x => x.CacheKey == cacheKey)
            .FirstOrDefaultAsync();

        var isAvailable = obj != null;
        Log.Verbose("IsAvailable CacheTower sqlite layer. Key: {CacheKey}. Result: {IsAvailable}", cacheKey, isAvailable);

        return isAvailable;
    }
}