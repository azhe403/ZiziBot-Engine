using CacheTower;
using Serilog;

namespace ZiziBot.Caching.Json;

public class JsonLayerProvider : ILocalCacheLayer
{
    private static readonly SemaphoreSlim SemaphoreSlim = new(1);
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

    public ValueTask EvictAsync(string cacheKey)
    {
        var jsonFile = CacheToFile(cacheKey: cacheKey);
        jsonFile.DeleteFile();

        return default;
    }

    public async ValueTask<CacheEntry<T>?> GetAsync<T>(string cacheKey)
    {
        var jsonFile = CacheToFile(cacheKey: cacheKey);

        if (!File.Exists(path: jsonFile))
            return null;

        var json = await File.ReadAllTextAsync(path: jsonFile);

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

        await SemaphoreSlim.WaitAsync();

        try
        {
            await File.WriteAllTextAsync(path: jsonFile, contents: content.ToJson(true));
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    public ValueTask<bool> IsAvailableAsync(string cacheKey)
    {
        var jsonFile = CacheToFile(cacheKey: cacheKey);
        if (File.Exists(path: jsonFile))
        {
            return new(true);
        }

        return default;
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
            cache.Path.DeleteFile();
        }
    }

    private string CacheToFile(string cacheKey)
    {
        var jsonFile = Path.Combine(path1: DirPath, path2: cacheKey + ".json");

        return jsonFile.EnsureDirectory();
    }
}