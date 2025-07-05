using CacheTower;

namespace ZiziBot.Database.Caching.Redis;

internal record struct RedisLayerOptions(
    ICacheSerializer Serializer,
    int DatabaseIndex = -1,
    string PrefixRoot = ""
);