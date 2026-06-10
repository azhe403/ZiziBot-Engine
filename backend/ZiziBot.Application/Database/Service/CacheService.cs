using CacheTower;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;
using ZiziBot.Common.Interfaces;
using ZiziBot.Common.Types;

namespace ZiziBot.Application.Database.Service;

public class CacheService(
    ILogger<CacheService> logger,
    IOptions<CacheConfig> cacheConfig,
    ICacheStack cacheStack,
    IFusionCache fusionCache
)
    : ICacheService
{
    private readonly CacheConfig _cacheConfig = cacheConfig.Value;
    private const string DefaultExpireAfter = "24h";
    private const string DefaultStaleAfter = "15s";

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
        {
            logger.LogDebug(
                "Cache bypassed. Key={CacheKey} Type={CacheType} DisableCache={DisableCache}",
                cacheKey,
                typeof(T).Name,
                disableCache
            );
            return await action();
        }

        if (evictBefore)
        {
            logger.LogDebug(
                "Cache evict-before requested. Key={CacheKey} Type={CacheType}",
                cacheKey,
                typeof(T).Name
            );
            await EvictAsync(cacheKey);
        }

        var resolvedExpireAfter = expireAfter ?? DefaultExpireAfter;
        var resolvedStaleAfter = staleAfter ?? DefaultStaleAfter;

        var expireAfterSpan = resolvedExpireAfter.ToTimeSpan();
        var staleAfterSpan = resolvedStaleAfter.ToTimeSpan();

        cacheKey = cacheKey.ForCacheKey();

        try
        {
            logger.LogDebug(
                "Cache resolve started. Key={CacheKey} Type={CacheType} Engine={CacheEngine} StaleAfter={StaleAfter} ExpireAfter={ExpireAfter} EvictBefore={EvictBefore} EvictAfter={EvictAfter}",
                cacheKey,
                typeof(T).Name,
                _cacheConfig.CacheEngine,
                staleAfterSpan,
                expireAfterSpan,
                evictBefore,
                evictAfter
            );

            var cacheSettings = new CacheSettings(expireAfterSpan, staleAfterSpan);

            var cache = await cacheStack.GetOrSetAsync<T>(
                cacheKey: cacheKey.Trim(),
                valueFactory: async (_) =>
                {
                    logger.LogDebug(
                        "Cache value factory invoked. Key={CacheKey} Type={CacheType} StaleAfter={StaleAfter} ExpireAfter={ExpireAfter}",
                        cacheKey,
                        typeof(T).Name,
                        staleAfterSpan,
                        expireAfterSpan
                    );

                    return await action();
                },
                settings: cacheSettings
            );

            logger.LogDebug(
                "Cache resolve completed. Key={CacheKey} Type={CacheType} StaleAfter={StaleAfter} ExpireAfter={ExpireAfter}",
                cacheKey,
                typeof(T).Name,
                staleAfterSpan,
                expireAfterSpan
            );

            if (evictAfter)
            {
                logger.LogDebug(
                    "Cache evict-after requested. Key={CacheKey} Type={CacheType}",
                    cacheKey,
                    typeof(T).Name
                );
                await EvictAsync(cacheKey);
            }

            return cache;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Cache resolve failed. Key={CacheKey} Type={CacheType} ThrowIfError={ThrowIfError}",
                cacheKey,
                typeof(T).Name,
                throwIfError
            );

            if (throwIfError)
                throw;

            logger.LogWarning(
                "Falling back to direct action after cache failure. Key={CacheKey} Type={CacheType}",
                cacheKey,
                typeof(T).Name
            );

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
            logger.LogDebug(
                "Using Fusion cache path. Key={CacheKey} Type={CacheType}",
                cacheKey,
                typeof(T).Name
            );
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
            logger.LogDebug("Cache eviction started. Key={CacheKey}", cacheKey);
            await cacheStack.EvictAsync(cacheKey);
            logger.LogDebug("Cache eviction completed. Key={CacheKey}", cacheKey);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Cache eviction failed. Key={CacheKey}", cacheKey);

            if (_cacheConfig.UseJsonFile)
            {
                logger.LogWarning(
                    "Deleting CacheTower JSON directory after eviction failure. Path={CachePath}",
                    PathConst.CACHE_TOWER_JSON
                );
                PathConst.CACHE_TOWER_JSON.DeleteDirectory();
            }
        }
    }

    private async ValueTask<TValue> FusionGetAsync<TValue>(
        string key,
        Func<CancellationToken, Task<TValue>> factory
    )
    {
        var cached = await fusionCache.GetOrSetAsync<TValue>(key: key, factory: async (ctx, ct) =>
        {
            logger.LogDebug(
                "Fusion cache value factory invoked. Key={CacheKey} Type={CacheType}",
                key,
                typeof(TValue).Name
            );

            return await factory(ct);
        });

        logger.LogDebug(
            "Fusion cache resolve completed. Key={CacheKey} Type={CacheType}",
            key,
            typeof(TValue).Name
        );

        return cached;
    }
}
