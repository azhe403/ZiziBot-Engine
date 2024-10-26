using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZiziBot.DataSource.MongoEf;

namespace ZiziBot.Infrastructure.MongoConfig;

public class MongoConfigSource(string connectionString) : IConfigurationSource
{
    readonly MongoEfContext _dbContext = new();

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new MongoConfigProvider(this);
    }

    public Dictionary<string, string?> GetAppSettings()
    {
        SeedAppSettings();

        var appSettingsList = _dbContext.AppSettings.AsNoTracking()
            .Where(x => x.Status == EventStatus.Complete)
            .ToList();

        return appSettingsList
            .Select(x => new KeyValuePair<string, string?>(x.Name, x.Value))
            .DistinctBy(pair => pair.Key)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    void SeedAppSettings()
    {
        var seedAppSettings = new List<SeedSettingDto> {
            new() {
                Root = "Engine",
                KeyPair = new() {
                    { "ProductName", "ZiziBot" },
                    { "Description", "ZiziBot is a Telegram bot that can help you manage your group." },
                    { "Vendor", "WinTenDev" },
                    { "Website", "https://winten.my.id" },
                    { "Support", "https://t.me/WinTenDevSupport" },
                    { "EnableChatRestriction", false },
                    { "TelegramEngineMode", "Auto" }
                }
            },
            new() {
                Root = "Log",
                KeyPair = new() {
                    { "ProcessEnrich", false }
                }
            },
            new() {
                Root = "Jwt",
                KeyPair = new() {
                    { "Key", "YOUR_SECURE_SECRET_KEY" },
                    { "Issuer", "YOUR_ISSUER" },
                    { "Audience", "YOUR_AUDIENCE" },
                    { "ExpireDays", 3 }
                }
            },
            new() {
                Root = "EventLog",
                KeyPair = new() {
                    { "ChatId", 12345 },
                    { "ThreadId", 34567 },
                    { "BackupDB", "0" },
                    { "Exception", 0 },
                    { "EventLog", 0 }
                }
            },
            new() {
                Root = "Hangfire",
                KeyPair = new() {
                    { "DashboardTitle", "Zizi Dev – Hangfire Dashboard" },
                    { "CurrentStorage", 2 },
                    { "MongoDbConnection", "mongo://localhost:21750" },
                    { "WorkerMultiplier", 2 },
                    { "Queues", "default" }
                }
            },
            new() {
                Root = "Flag",
                KeyPair = new() {
                    { "IsEnabled", false },
                    { "IsForwardMessageEnabled", false }
                }
            },
            new() {
                Root = "Cache",
                KeyPair = new() {
                    { "UseJsonFile", false },
                    { "UseFirebase", false },
                    { "UseMongoDb", false },
                    { "UseRedis", false },
                    { "UseSqlite", false },
                    { "PrefixRoot", "zizi_dev" },
                    { "RedisConnection", "localhost:6379" }
                }
            },
            new() {
                Root = "Sentry",
                KeyPair = new() {
                    { "IsEnabled", false },
                    { "Dsn", "SENTRY_DSN" }
                }
            },
            new() {
                Root = "Mirror",
                KeyPair = new() {
                    { "ApprovalChannelId", "-969706112" },
                    { "TrakteerVerificationApi", "" },
                    { "SaweriaVerificationApi", "" },
                    { "UseCustomTrakteerApi", false },
                    { "UseCustomSaweriaApi", false },
                    { "PaymentExpirationDays", 3 }
                }
            },
            new() {
                Root = "Gcp",
                KeyPair = new() {
                    { "IsEnabled", false },
                    { "FirebaseProjectUrl", "https://yourapp.firebaseio.com" },
                    { "FirebaseServiceAccountJson", "{\"your_firebase_service_account_json\":\"string\"}" }
                }
            },
            new() {
                Root = "OptiicDev",
                KeyPair = new() {
                    { "ApiKey", "YOUR_API_KEY" }
                }
            },
            new() {
                Root = "BinderByte",
                KeyPair = new() {
                    { "IsEnabled", false },
                    { "BaseUrl", "https://api.binderbyte.com" },
                    { "ApiKey", "YOUR_API_KEY" }
                }
            }
        };

        var appSettings = _dbContext.AppSettings.AsNoTracking()
            .Where(x => x.Status == EventStatus.Complete)
            .ToList();

        var appSettingsDictionary = appSettings.ToDictionary(x => x.Name, x => x.Value.ToString());
        var diffSeedAppSetting = seedAppSettings
            .SelectMany(s => s.KeyPair, (s, kv) => new {
                Root = s.Root,
                Key = $"{s.Root}:{kv.Key}",
                Value = kv.Value.ToString()
            })
            .Where(kv => !appSettingsDictionary.ContainsKey(kv.Key))
            .ToList();

        var transactionId = Guid.NewGuid().ToString();

        diffSeedAppSetting.ForEach(seed => {
                var appSetting = _dbContext.AppSettings.AsNoTracking()
                    .Where(x => x.Status == EventStatus.Complete)
                    .FirstOrDefault(settings => settings.Name == seed.Key);

                if (appSetting != null) return;

                _dbContext.AppSettings.Add(new() {
                    Root = seed.Root,
                    Name = seed.Key,
                    Field = seed.Key,
                    DefaultValue = $"{seed.Value}",
                    InitialValue = $"{seed.Value}",
                    Value = $"{seed.Value}",
                    TransactionId = transactionId,
                    Status = EventStatus.Complete
                });
            }
        );

        _dbContext.SaveChanges();
    }
}