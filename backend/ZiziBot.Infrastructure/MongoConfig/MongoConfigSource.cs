using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZiziBot.Common.Utils;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;
using ZiziBot.TelegramBot.Framework.Models.Enums;
using ExecutionStrategy = ZiziBot.TelegramBot.Framework.Models.Enums.ExecutionStrategy;

namespace ZiziBot.Infrastructure.MongoConfig;

public class MongoConfigSource(string connectionString) : IConfigurationSource
{
    private readonly MongoEfContext _dbContext = new();

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
        var engine = new EngineConfig {
            ProductName = "ZiziBot",
            Description = "ZiziBot is a Telegram bot that can help you manage your group.",
            Vendor = "@AZCorp",
            Website = "https://azhe.my.id",
            Support = "https://azhe.my.id",
            TelegramEngineMode = BotEngineMode.Auto,
            ExecutionStrategy = ExecutionStrategy.Await,
            EnableChatRestriction = false
        };

        var jwt = new JwtConfig() {
            Key = "YOUR_SECURE_SECRET_KEY",
            Audience = "YOUR_AUDIENCE",
            Issuer = "YOUR_ISSUER",
        };

        var eventLog = new EventLogConfig() {
            ChatId = 12345,
            ThreadId = 34567,
            BackupDB = 0,
            Exception = 0,
            EventLog = 0,
        };

        var cache = new CacheConfig() {
            PrefixRoot = "zizibot",
            UseJsonFile = false,
            UseFirebase = false,
            UseMongoDb = false,
            UseRedis = false,
            UseSqlite = false,
            RedisConnection = "localhost:6379"
        };

        var hangfire = new HangfireConfig() {
            CurrentStorage = CurrentStorage.InMemory,
            DashboardTitle = "Zizi Dev - Hangfire Dashboard",
            MongoDbConnection = "mongo://localhost:21750",
            WorkerMultiplier = 2,
            Queues = "default"
        };

        var mirror = new MirrorConfig() {
            ApprovalChannelId = 12345,
            PaymentExpirationDays = 3,
            SaweriaVerificationApi = "",
            TrakteerVerificationApi = "",
            TrakteerWebHookToken = "YOUR_TRAKTEER_WEBHOOK_TOKEN",
            UseCustomSaweriaApi = false,
            UseCustomTrakteerApi = false,
        };

        var pendekin = new PendekinConfig() {
            RouterBaseUrl = "127.0.0.1:7140"
        };

        var sentry = new SentryConfig() {
            IsEnabled = false,
            Dsn = "SENTRY_DSN"
        };

        var gcp = new GcpConfig() {
            IsEnabled = false,
            FirebaseProjectUrl = "yourapp.firebaseio.com",
            FirebaseServiceAccountJson = "{\"your_firebase_service_account_json\":\"string\"}"
        };

        var optiicDev = new OptiicDevConfig() {
            IsEnabled = false,
            ApiKey = "YOUR_API_KEY"
        };

        var binderByte = new BinderByteConfig() {
            IsEnabled = false,
            BaseUrl = "api.your-domain.com",
            ApiKey = "YOUR_API_KEY"
        };

        var seedAppSettings = new List<SeedSettingDto> {
            new() {
                Root = nameof(ConfigRoot.Engine),
                KeyPair = engine.ToDictionary(StringType.PascalCase)
            },
            new() {
                Root = nameof(ConfigRoot.EventLog),
                KeyPair = new() {
                    { "ProcessEnrich", false }
                }
            },
            new() {
                Root = nameof(ConfigRoot.Jwt),
                KeyPair = jwt.ToDictionary(StringType.PascalCase)
            },
            new() {
                Root = nameof(ConfigRoot.EventLog),
                KeyPair = eventLog.ToDictionary(StringType.PascalCase)
            },
            new() {
                Root = nameof(ConfigRoot.Hangfire),
                KeyPair = hangfire.ToDictionary(StringType.PascalCase)
            },
            new() {
                Root = "Flag",
                KeyPair = new() {
                    { "IsEnabled", false },
                    { "IsForwardMessageEnabled", false }
                }
            },
            new() {
                Root = nameof(ConfigRoot.Cache),
                KeyPair = cache.ToDictionary(StringType.PascalCase)
            },
            new() {
                Root = nameof(ConfigRoot.Sentry),
                KeyPair = sentry.ToDictionary(StringType.PascalCase)
            },
            new() {
                Root = nameof(ConfigRoot.Mirror),
                KeyPair = mirror.ToDictionary(StringType.SnakeCase)
            },
            new() {
                Root = nameof(ConfigRoot.Pendekin),
                KeyPair = pendekin.ToDictionary(StringType.PascalCase)
            },
            new() {
                Root = nameof(ConfigRoot.Gcp),
                KeyPair = gcp.ToDictionary(StringType.PascalCase)
            },
            new() {
                Root = nameof(ConfigRoot.OptiicDev),
                KeyPair = optiicDev.ToDictionary(StringType.PascalCase)
            },
            new() {
                Root = nameof(ConfigRoot.BinderByte),
                KeyPair = binderByte.ToDictionary(StringType.PascalCase)
            }
        };

        var appSettings = _dbContext.AppSettings.AsNoTracking()
            .Where(x => x.Status == EventStatus.Complete)
            .Select(x => new {
                Root = x.Root,
                Name = x.Name,
                Value = x.Value
            })
            .ToList();

        var appSettingsDictionary = appSettings.ToDictionary(x => x.Name, x => x.Value.ToString());
        var seedableAppSetting = seedAppSettings
            .SelectMany(s => s.KeyPair, (s, kv) => new {
                Root = s.Root,
                Key = $"{s.Root}:{kv.Key}",
                Value = kv.Value.ToString()
            })
            .Where(kv => !appSettingsDictionary.ContainsKey(kv.Key))
            .ToList();

        var transactionId = Guid.NewGuid().ToString();

        seedableAppSetting.ForEach(seed => {
            var appSetting = _dbContext.AppSettings.AsNoTracking()
                .Where(x => x.Status == EventStatus.Complete)
                .FirstOrDefault(settings => settings.Name == seed.Key);

            if (appSetting != null) return;

            _dbContext.AppSettings.Add(new AppSettingsEntity {
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