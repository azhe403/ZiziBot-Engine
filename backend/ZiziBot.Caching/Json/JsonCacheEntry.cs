﻿namespace ZiziBot.Caching.Json;

public class JsonCacheEntry<TValue>
{
    public required string CacheKey { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public TValue? CacheValue { get; set; }
}