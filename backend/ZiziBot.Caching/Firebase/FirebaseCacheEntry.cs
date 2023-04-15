namespace ZiziBot.Caching.Firebase;

public class FirebaseCacheEntry
{
    public string CacheKey { get; set; }
    public object? Value { get; set; }
    public DateTime Expiry { get; set; }
}