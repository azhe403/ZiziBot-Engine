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

    public Task EvictAsync(string cacheKey);
}