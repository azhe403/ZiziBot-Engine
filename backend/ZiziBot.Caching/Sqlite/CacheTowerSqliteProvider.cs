using CacheTower;
using Microsoft.EntityFrameworkCore;

namespace ZiziBot.Caching.Sqlite;

public class CacheTowerSqliteProvider : ICacheLayer
{
    private readonly SqliteCachingDbContext _dbContext;

    public CacheTowerSqliteProvider(string dbPath)
    {
        _dbContext = new SqliteCachingDbContext(dbPath);

        _dbContext.Database.EnsureCreated();
    }

    public async ValueTask FlushAsync()
    {
        await _dbContext.SqliteCache.ExecuteDeleteAsync();
        await _dbContext.SaveChangesAsync();
    }

    public async ValueTask CleanupAsync()
    {
        await _dbContext.SqliteCache.Where(x => x.Expiry < DateTime.UtcNow).ExecuteDeleteAsync();
        await _dbContext.SaveChangesAsync();
    }

    public async ValueTask EvictAsync(string cacheKey)
    {
        await _dbContext.SqliteCache.Where(x => x.CacheKey == cacheKey).ExecuteDeleteAsync();
        await _dbContext.SaveChangesAsync();
    }

    public async ValueTask<CacheEntry<T>?> GetAsync<T>(string cacheKey)
    {
        var cacheEntry = default(CacheEntry<T>);

        var obj = await _dbContext.SqliteCache.Where(x => x.CacheKey == cacheKey).FirstOrDefaultAsync();

        if (obj != null)
        {
            var value = obj.Value.ToObject<T>();
            return new CacheEntry<T>(value, obj.Expiry);
        }

        return cacheEntry;
    }

    public async ValueTask SetAsync<T>(string cacheKey, CacheEntry<T?> cacheEntry)
    {
        var findCache = await _dbContext.SqliteCache.FirstOrDefaultAsync(x => x.CacheKey == cacheKey);
        if (findCache == null)
        {
            await _dbContext.SqliteCache.AddAsync(new SqliteCacheEntity()
            {
                CacheKey = cacheKey,
                Value = cacheEntry.Value.ToJson(),
                Expiry = cacheEntry.Expiry
            });
        }
        else
        {
            findCache.Value = cacheEntry.Value.ToJson();
            findCache.Expiry = cacheEntry.Expiry;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async ValueTask<bool> IsAvailableAsync(string cacheKey)
    {
        var obj = await _dbContext.SqliteCache.Where(x => x.CacheKey == cacheKey).FirstOrDefaultAsync();
        return obj != null;
    }
}