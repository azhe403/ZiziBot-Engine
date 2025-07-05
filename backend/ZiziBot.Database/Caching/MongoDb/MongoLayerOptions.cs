using CacheTower;

namespace ZiziBot.Database.Caching.MongoDb;

internal record struct MongoLayerOptions(
    string ConnectionString,
    ICacheSerializer Serializer
);