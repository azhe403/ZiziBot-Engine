using CacheTower;

namespace ZiziBot.Application.Database.CacheTower.MongoDb;

internal record struct MongoLayerOptions(
    string ConnectionString,
    ICacheSerializer Serializer
);
