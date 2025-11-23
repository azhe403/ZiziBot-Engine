using CacheTower;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;
using ZiziBot.Common.Interfaces;
using ZiziBot.Common.Types;

namespace ZiziBot.Database.Service;

public class CacheService(
    ILogger<CacheService> logger,
    IOptions<CacheConfig> cacheConfig,
    ICacheStack cacheStack,
    IFusionCache fusionCache
)
    : ICacheService
{
    private readonly CacheConfig _cacheConfig = cacheConfig.Value;
    private string _expireAfter = "24h";
    private string _staleAfter = "15s";

    public async Task<T> GetOrSetAsync<T>(
        string cacheKey,
        Func<Task<T>> action,
        bool disableCache = false,
        bool evictBefore = false,
        bool evictAfter = false,
        string? expireAfter = null,
        string? staleAfter = null,
        bool throwIfError = false
    )
    {
        if (disableCache)
            return await action();

        if (evictBefore)
            await EvictAsync(cacheKey);

        if (expireAfter != null) _expireAfter = expireAfter;
        if (staleAfter != null) _staleAfter = staleAfter;

        var expireAfterSpan = _expireAfter.ToTimeSpan();
        var staleAfterSpan = _staleAfter.ToTimeSpan();

        cacheKey = cacheKey.ForCacheKey();

        try
        {
            logger.LogDebug("Loading Cache with Key: {CacheKey}. StaleAfter: {StaleAfter}. ExpireAfter: {ExpireAfter}", cacheKey, staleAfterSpan, expireAfterSpan);

            var cacheSettings = new CacheSettings(expireAfterSpan, staleAfterSpan);

            var cache = await cacheStack.GetOrSetAsync<T>(
                cacheKey: cacheKey.Trim(),
                valueFactory: async (_) =>
                {
                    logger.LogDebug("Updating cache with Key: {CacheKey}. StaleAfter: {StaleAfter}. ExpireAfter: {ExpireAfter}", cacheKey, staleAfterSpan, expireAfterSpan);

                    return await action();
                },
                settings: cacheSettings
            );

            logger.LogDebug("Loaded Cache with Key: {CacheKey}. StaleAfter: {StaleAfter}. ExpireAfter: {ExpireAfter}", cacheKey, staleAfterSpan, expireAfterSpan);

            if (evictAfter)
                await EvictAsync(cacheKey);

            return cache;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error loading cache with Key: {Key}", cacheKey);

            if (throwIfError)
                throw;

            await EvictAsync(cacheKey);

            var data = await action();

            return data;
        }
    }

    public Task<T> GetOrSetAsync<T>(CacheParam<T> cacheParam)
    {
        return GetOrSetAsync(cacheParam.CacheKey,
            cacheParam.Action,
            cacheParam.DisableCache,
            cacheParam.EvictBefore,
            cacheParam.EvictAfter,
            cacheParam.ExpireAfter,
            cacheParam.StaleAfter,
            cacheParam.ThrowIfError
        );
    }

    public async Task<T?> GetOrSetAsyncV2<T>(
        string cacheKey,
        Func<Task<CacheReturn<T>>> action,
        bool disableCache = false,
        bool evictBefore = false,
        bool evictAfter = false,
        string? expireAfter = null,
        string? staleAfter = null,
        bool throwIfError = false
    )
    {
        if (_cacheConfig.CacheEngine == CacheEngine.FusionCache)
        {
            var fusion = await FusionGetAsync(cacheKey, async (x) => await action());
            return fusion.Data;
        }

        var cache = await GetOrSetAsync(cacheKey: cacheKey,
            action: action,
            disableCache: disableCache,
            evictBefore: evictBefore,
            evictAfter: evictAfter,
            expireAfter: expireAfter,
            staleAfter: staleAfter,
            throwIfError: throwIfError);

        return cache.Data;
    }


    public Task<TData?> GetOrSetAsyncV2<TData>(CacheV2Param<TData> cacheParam)
    {
        return GetOrSetAsyncV2(
            cacheKey: cacheParam.CacheKey,
            action: cacheParam.Action,
            disableCache: cacheParam.DisableCache,
            evictBefore: cacheParam.EvictBefore,
            evictAfter: cacheParam.EvictAfter,
            expireAfter: cacheParam.ExpireAfter,
            staleAfter: cacheParam.StaleAfter,
            throwIfError: cacheParam.ThrowIfError
        );
    }

    public async Task EvictAsync(string cacheKey)
    {
        try
        {
            logger.LogDebug("Evicting cache with key: {CacheKey}", cacheKey);
            await cacheStack.EvictAsync(cacheKey);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Fail to evict cache Key: {Key}", cacheKey);

            if (_cacheConfig.UseJsonFile)
                PathConst.CACHE_TOWER_JSON.DeleteDirectory();
        }
    }

    private async ValueTask<TValue> FusionGetAsync<TValue>(
        string key,
        Func<CancellationToken, Task<TValue>> factory
    )
    {
        var cached = await fusionCache.GetOrSetAsync<TValue>(key: key, factory: async (ctx, ct) =>
        {
            logger.LogDebug("Loading Fusion cache with key: {Key}", key);

            return await factory(ct);
        });

        return cached;
    }
}