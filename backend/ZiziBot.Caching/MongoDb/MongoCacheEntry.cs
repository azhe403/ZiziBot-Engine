using MongoDB.Bson.Serialization.Attributes;

namespace ZiziBot.Caching.MongoDb;

internal class MongoCacheEntry
{
    [BsonId]
    public string Id { get; set; }

    public string CacheKey { get; set; }
    public DateTime CreatedDate { get; set; }
    public object? Value { get; set; }
    public DateTime Expiry { get; set; }
}