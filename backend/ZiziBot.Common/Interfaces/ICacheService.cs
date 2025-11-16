using ZiziBot.Common.Types;

namespace ZiziBot.Common.Interfaces;

public interface ICacheService
{
    public Task<T> GetOrSetAsync<T>(
        string cacheKey,
        Func<Task<T>> action,
        bool disableCache = false,
        bool evictBefore = false,
        bool evictAfter = false,
        string? expireAfter = null,
        string? staleAfter = null,
        bool throwIfError = false
    );

    public Task<T> GetOrSetAsync<T>(CacheParam<T> cacheParam);

    public Task<TData?> GetOrSetAsyncV2<TData>(
        string cacheKey,
        Func<Task<CacheReturn<TData>>> action,
        bool disableCache = false,
        bool evictBefore = false,
        bool evictAfter = false,
        string? expireAfter = null,
        string? staleAfter = null,
        bool throwIfError = false
    );

    public Task<TData?> GetOrSetAsyncV2<TData>(CacheV2Param<TData> cacheParam);

    public Task EvictAsync(string cacheKey);
}