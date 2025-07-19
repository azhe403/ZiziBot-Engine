using CacheTower;

namespace ZiziBot.Database.CacheTower.Redis;

internal record struct RedisLayerOptions(
    ICacheSerializer Serializer,
    int DatabaseIndex = -1,
    string PrefixRoot = ""
);