using Microsoft.Extensions.Configuration;

namespace ZiziBot.Infrastructure.MongoConfig;

public class MongoConfigSource : IConfigurationSource
{
    private readonly MongoDbContextBase _dbContext;

    public MongoConfigSource(string connectionString)
    {
        _dbContext = new MongoDbContextBase(connectionString);
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new MongoConfigProvider(this);
    }

    public Dictionary<string, string?> GetAppSettings()
    {
        SeedAppSettings();

        var appSettingsList = _dbContext.AppSettings.ToList();

        return appSettingsList
            .Select(x => new KeyValuePair<string, string?>(x.Name, x.Value))
            .DistinctBy(pair => pair.Key)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    private void SeedAppSettings()
    {
        var listAppSettings = new List<SeedSettingDto>
        {
            new()
            {
                Root = "Engine",
                KeyPair = new Dictionary<string, object>()
                {
                    { "ProductName", "ZiziBot" },
                    { "Description", "ZiziBot is a Telegram bot that can help you manage your group." },
                    { "Vendor", "WinTenDev" },
                    { "Website", "https://winten.my.id" },
                    { "Support", "https://t.me/WinTenDevSupport" },
                    { "EnableChatRestriction", false },
                    { "TelegramEngineMode", "Auto" }
                }
            },
            new()
            {
                Root = "Log",
                KeyPair = new Dictionary<string, object>
                {
                    { "ProcessEnrich", false }
                }
            },
            new()
            {
                Root = "Jwt",
                KeyPair = new Dictionary<string, object>
                {
                    { "Key", "YOUR_SECURE_SECRET_KEY" },
                    { "Issuer", "YOUR_ISSUER" },
                    { "Audience", "YOUR_AUDIENCE" },
                    { "ExpireDays", 3 },
                }
            },
            new()
            {
                Root = "EventLog",
                KeyPair = new Dictionary<string, object>
                {
                    { "ChatId", 12345 },
                    { "ThreadId", 34567 },
                    { "BackupDB", "0" },
                    { "Exception", 0 },
                    { "EventLog", 0 }
                }
            },
            new()
            {
                Root = "Hangfire",
                KeyPair = new Dictionary<string, object>
                {
                    { "CurrentStorage", 2 },
                    { "MongoDbConnection", "mongo://localhost:21750" },
                    { "WorkerMultiplier", 2 },
                    { "Queues", "default" }
                }
            },
            new()
            {
                Root = "Flag",
                KeyPair = new Dictionary<string, object>()
                {
                    { "IsEnabled", false },
                    { "IsForwardMessageEnabled", false }
                }
            },
            new()
            {
                Root = "Cache",
                KeyPair = new Dictionary<string, object>
                {
                    { "UseJsonFile", false },
                    { "UseFirebase", false },
                    { "UseRedis", false },
                    { "UseSqlite", false },
                    { "RedisConnection", "localhost:6379" }
                }
            },
            new()
            {
                Root = "Sentry",
                KeyPair = new Dictionary<string, object>()
                {
                    { "IsEnabled", false },
                    { "Dsn", "SENTRY_DSN" }
                }
            },
            new()
            {
                Root = "Mirror",
                KeyPair = new Dictionary<string, object>()
                {
                    { "ApprovalChannelId", "-969706112" },
                    { "TrakteerVerificationApi", "" },
                    { "PaymentExpirationDays", 3 }
                }
            },
            new()
            {
                Root = "Gcp",
                KeyPair = new Dictionary<string, object>
                {
                    { "IsEnabled", false },
                    { "FirebaseProjectUrl", "https://yourapp.firebaseio.com" },
                    { "FirebaseServiceAccountJson", "{\"your_firebase_service_account_json\":\"string\"}" }
                }
            },
            new()
            {
                Root = "OptiicDev",
                KeyPair = new Dictionary<string, object>
                {
                    { "ApiKey", "YOUR_API_KEY" }
                }
            },
            new()
            {
                Root = "BinderByte",
                KeyPair = new Dictionary<string, object>
                {
                    { "IsEnabled", false },
                    { "BaseUrl", "https://api.binderbyte.com" },
                    { "ApiKey", "YOUR_API_KEY" },
                }
            }
        };

        listAppSettings.ForEach(
            dto => {
                foreach (var config in dto.KeyPair)
                {
                    var prefix = dto.Root;
                    var field = $"{prefix}:{config.Key}";

                    var appSetting = _dbContext.AppSettings.FirstOrDefault(settings => settings.Name == field);

                    if (appSetting != null) continue;

                    _dbContext.AppSettings.Add(new AppSettingsEntity()
                    {
                        Name = field,
                        Field = field,
                        Value = $"{config.Value}",
                        DefaultValue = $"{config.Value}",
                        TransactionId = Guid.NewGuid().ToString(),
                        Status = (int)EventStatus.Complete
                    });
                }
            }
        );

        _dbContext.SaveChanges();
    }
}