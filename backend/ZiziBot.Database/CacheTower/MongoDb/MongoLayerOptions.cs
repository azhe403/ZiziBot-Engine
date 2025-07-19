using CacheTower;

namespace ZiziBot.Database.CacheTower.MongoDb;

internal record struct MongoLayerOptions(
    string ConnectionString,
    ICacheSerializer Serializer
);