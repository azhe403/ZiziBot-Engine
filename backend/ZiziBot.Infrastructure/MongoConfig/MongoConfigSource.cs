using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZiziBot.Database.MongoDb;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Infrastructure.MongoConfig;

public class MongoConfigSource(string connectionString) : IConfigurationSource
{
    private readonly MongoDbContext _dbContext = new();

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new MongoConfigProvider(this);
    }

    public Dictionary<string, string?> GetAppSettings()
    {
        SeedAppSettings();

        var appSettingsList = _dbContext.AppSettings.AsNoTracking()
            .Where(x => x.Status == EventStatus.Complete)
            .Select(x => new KeyValuePair<string, string?>(x.Name, x.Value))
            .ToList();

        return appSettingsList
            .DistinctBy(pair => pair.Key)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    private void SeedAppSettings()
    {
        var defaultAppSettings = ConfigUtil.GetAppDefault();
        var appSettings = _dbContext.AppSettings.AsNoTracking()
            .Where(x => x.Status == EventStatus.Complete)
            .Select(x => new
            {
                Root = x.Root,
                Name = x.Name,
                Value = x.Value
            })
            .ToList();

        var appSettingsDictionary = appSettings.ToDictionary(x => x.Name, x => x.Value.ToString());
        var seedableAppSetting = defaultAppSettings
            .SelectMany(s => s.KeyPair, (s, kv) => new
            {
                Root = s.Root,
                Key = $"{s.Root}:{kv.Key}",
                Value = kv.Value.ToString(),
                ValueType = kv.Value.GetType()
            })
            .Where(kv => !appSettingsDictionary.ContainsKey(kv.Key))
            .ToList();

        var transactionId = Guid.NewGuid().ToString();

        seedableAppSetting.ForEach(seed =>
        {
            var appSetting = _dbContext.AppSettings.AsNoTracking()
                .Where(x => x.Status == EventStatus.Complete)
                .FirstOrDefault(settings => settings.Name == seed.Key);

            if (appSetting != null) return;

            _dbContext.AppSettings.Add(new AppSettingsEntity
            {
                Root = seed.Root,
                Name = seed.Key,
                Field = seed.Key,
                DefaultValue = $"{seed.Value}",
                InitialValue = $"{seed.Value}",
                Value = $"{seed.Value}",
                TransactionId = transactionId,
                Status = EventStatus.Complete
            });
        });

        _dbContext.SaveChanges();
    }
}