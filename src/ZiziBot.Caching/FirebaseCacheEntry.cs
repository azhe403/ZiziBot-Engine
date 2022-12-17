namespace ZiziBot.Caching;

public class FirebaseCacheEntry
{
    public string CacheKey { get; set; }
    public object Value { get; set; }
    public DateTime Expiry { get; set; }
}