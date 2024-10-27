using CacheTower;
using Microsoft.EntityFrameworkCore;

namespace ZiziBot.Caching.Sqlite;

internal class SqliteLayerProvider : ICacheLayer
{
    private readonly string _dbPath;

    public SqliteLayerProvider(string dbPath)
    {
        _dbPath = dbPath;

        GetContext().Database.EnsureCreated();
    }

    public async ValueTask FlushAsync()
    {
        var dbContext = GetContext();

        await dbContext.SqliteCache.ExecuteDeleteAsync();
        await dbContext.SaveChangesAsync();
    }

    public async ValueTask CleanupAsync()
    {
        var dbContext = GetContext();

        await dbContext.SqliteCache.Where(x => x.ExpiryDate < DateTime.UtcNow).ExecuteDeleteAsync();
        await dbContext.SaveChangesAsync();
    }

    public async ValueTask EvictAsync(string cacheKey)
    {
        var dbContext = GetContext();

        await dbContext.SqliteCache.Where(x => x.CacheKey == cacheKey).ExecuteDeleteAsync();
        await dbContext.SaveChangesAsync();
    }

    public async ValueTask<CacheEntry<T>?> GetAsync<T>(string cacheKey)
    {
        var cacheEntry = default(CacheEntry<T>);
        var dbContext = GetContext();

        var obj = await dbContext.SqliteCache.Where(x => x.CacheKey == cacheKey).FirstOrDefaultAsync();

        if (obj != null)
        {
            var value = obj.Value.ToObject<T>();
            return new CacheEntry<T>(value, obj.ExpiryDate);
        }

        return cacheEntry;
    }

    public async ValueTask SetAsync<T>(string cacheKey, CacheEntry<T> cacheEntry)
    {
        var dbContext = GetContext();

        var findCache = await dbContext.SqliteCache.FirstOrDefaultAsync(x => x.CacheKey == cacheKey);
        if (findCache == null)
        {
            await dbContext.SqliteCache.AddAsync(new SqliteCacheEntity() {
                CacheKey = cacheKey,
                ExpiryDate = cacheEntry.Expiry,
                CreatedDate = DateTime.UtcNow,
                Value = cacheEntry.Value.ToJson(),
            });
        }
        else
        {
            findCache.ExpiryDate = cacheEntry.Expiry;
            findCache.Value = cacheEntry.Value.ToJson();
        }

        await dbContext.SaveChangesAsync();
    }

    public async ValueTask<bool> IsAvailableAsync(string cacheKey)
    {
        var dbContext = GetContext();

        var obj = await dbContext.SqliteCache.AsNoTracking()
            .Where(x => x.CacheKey == cacheKey)
            .FirstOrDefaultAsync();

        return obj != null;
    }

    private SqliteCachingDbContext GetContext()
    {
        var dbContext = new SqliteCachingDbContext(_dbPath);

        return dbContext;
    }
}