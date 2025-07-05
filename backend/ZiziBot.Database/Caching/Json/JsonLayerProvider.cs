using CacheTower;
using Serilog;
using ZiziBot.Common.Utils;

namespace ZiziBot.Database.Caching.Json;

public class JsonLayerProvider : ILocalCacheLayer
{
    public required string DirPath { get; set; }

    public required ICacheSerializer Serializer { get; set; }

    public async ValueTask FlushAsync()
    {
        await CleanupCache(true);
    }

    public async ValueTask CleanupAsync()
    {
        await CleanupCache();
    }

    public async ValueTask EvictAsync(string cacheKey)
    {
        var jsonFile = CacheToFile(cacheKey: cacheKey);
        await jsonFile.DeleteThreadSafeAsync();
    }

    public async ValueTask<CacheEntry<T>?> GetAsync<T>(string cacheKey)
    {
        var jsonFile = CacheToFile(cacheKey: cacheKey);

        if (!File.Exists(path: jsonFile))
            return null;

        var json = await jsonFile.ReadAllTextThreadSafeAsync();

        var content = json.ToObject<JsonCacheEntry<T>>();
        if (content == null)
            return null;

        return new(Value: content.CacheValue, Expiry: content.ExpireDate);
    }

    public async ValueTask SetAsync<T>(string cacheKey, CacheEntry<T> cacheEntry)
    {
        var jsonFile = CacheToFile(cacheKey: cacheKey);
        var content = new JsonCacheEntry<T> {
            CacheKey = cacheKey,
            CreatedDate = DateTime.UtcNow,
            ExpireDate = cacheEntry.Expiry,
            CacheValue = cacheEntry.Value
        };

        await jsonFile.WriteAllTextThreadSafeAsync(content.ToJson());
    }

    public async ValueTask<bool> IsAvailableAsync(string cacheKey)
    {
        await Task.Delay(1);

        var jsonFile = CacheToFile(cacheKey: cacheKey);
        var isAvailable = File.Exists(path: jsonFile);
        Log.Verbose("IsAvailable CacheTower json layer. Key: {CacheKey}. Result: {IsAvailable}", cacheKey, isAvailable);

        return isAvailable;
    }

    private async Task CleanupCache(bool any = false)
    {
        await Task.Delay(1);

        var data = DirPath.GetFiles().Select(x => new {
            Path = x.FullName,
            CacheEntry = File.ReadAllText(x.FullName).ToObject<JsonCacheEntry<object>>()
        }).ToList();

        foreach (var cache in data)
        {
            var cacheEntry = cache.CacheEntry;
            if (cacheEntry == null)
                continue;

            Log.Debug("Checking JSON cache: {Path}, ExpireDate: {ExpireDate}", cache.Path, cacheEntry.ExpireDate);
            if (cacheEntry.ExpireDate > DateTime.UtcNow && !any)
                continue;

            Log.Debug("Deleting JSON cache: {Path}", cache.Path);
            await cache.Path.DeleteThreadSafeAsync();
        }
    }

    private string CacheToFile(string cacheKey)
    {
        var jsonFile = Path.Combine(path1: DirPath, path2: cacheKey, ".json");

        return jsonFile.EnsureDirectory();
    }
}