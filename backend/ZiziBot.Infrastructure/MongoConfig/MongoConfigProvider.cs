using Microsoft.Extensions.Configuration;

namespace ZiziBot.Infrastructure.MongoConfig;

public class MongoConfigProvider : ConfigurationProvider
{
    private readonly MongoConfigSource _mongoConfigSource;

    public MongoConfigProvider(MongoConfigSource mongoConfigSource)
    {
        _mongoConfigSource = mongoConfigSource;
    }

    public override void Load()
    {
        Data = _mongoConfigSource.GetAppSettings();
    }
}