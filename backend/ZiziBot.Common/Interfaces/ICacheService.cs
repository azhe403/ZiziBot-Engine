using ZiziBot.Common.Types;

namespace ZiziBot.Common.Interfaces;

public interface ICacheService
{
    [Obsolete("Use GetOrSetAsync with Cache<T> instead")]
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

    public Task<T> GetOrSetAsync<T>(Cache<T> cache);

    public Task EvictAsync(string cacheKey);
}