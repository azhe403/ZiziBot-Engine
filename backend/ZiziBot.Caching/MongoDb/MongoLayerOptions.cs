using CacheTower;

namespace ZiziBot.Caching.MongoDb;

internal record struct MongoLayerOptions(
    string ConnectionString,
    ICacheSerializer Serializer
);