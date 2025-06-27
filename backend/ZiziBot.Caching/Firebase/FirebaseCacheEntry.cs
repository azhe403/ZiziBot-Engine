namespace ZiziBot.Caching.Firebase;

internal class FirebaseCacheEntry<T>
{
    public required string CacheKey { get; set; }
    public DateTime Expiry { get; set; }
    public T? Value { get; set; }
}