using Microsoft.Extensions.Configuration;
using MongoFramework;

namespace ZiziBot.Infrastructure.MongoConfig;

public class MongoConfigSource : IConfigurationSource
{
    private readonly string _connectionString;

    public MongoConfigSource(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new MongoConfigProvider(this);
    }

    public Dictionary<string, string?> GetAppSettings()
    {
        var dbContext = new AppSettingsDbContext(MongoDbConnection.FromConnectionString(_connectionString));
        var appSettingsList = dbContext.AppSettings.ToList();

        return appSettingsList
            .Select(x => new KeyValuePair<string, string?>(x.Name, x.Value))
            .DistinctBy(pair => pair.Key)
            .ToDictionary(x => x.Key, x => x.Value);
    }
}