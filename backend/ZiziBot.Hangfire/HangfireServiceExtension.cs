using Hangfire.Server;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace ZiziBot.Hangfire;

public static class HangfireServiceExtension
{
    public static IServiceCollection ConfigureHangfire(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Hangfire");
        var hangfireConfig = serviceProvider.GetRequiredService<IOptionsSnapshot<HangfireConfig>>().Value;

        var queues = new[] {
            "default",
            CronJobKey.Queue_Data,
            CronJobKey.Queue_Rss,
            CronJobKey.Queue_ShalatTime
        };

        JobStorage jobStorage = hangfireConfig.CurrentStorage switch {
            CurrentStorage.MongoDb => hangfireConfig.MongoDbConnection.ToMongoDbStorage(),
            CurrentStorage.Sqlite => new SQLiteStorage(PathConst.HANGFIRE_SQLITE_PATH.EnsureDirectory()),
            // CurrentStorage.LiteDb => new LiteDbStorage(PathConst.HANGFIRE_LITEDB_PATH.EnsureDirectory()),
            _ => new InMemoryStorage(new InMemoryStorageOptions())
        };

        services.AddHangfire(
            configuration => {
                configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseDarkDashboard()
                    .UseStorage(jobStorage)
                    .UseHeartbeatPage(checkInterval: TimeSpan.FromSeconds(3))
                    .UseMediatR();
            }
        );

        services.AddHangfireServer(
            optionsAction: (provider, options) => {
                options.WorkerCount = Environment.ProcessorCount * hangfireConfig.WorkerMultiplier;
                options.ServerTimeout = TimeSpan.FromMinutes(10);
                options.Queues = queues;
            },
            storage: jobStorage,
            additionalProcesses: new IBackgroundProcess[] {
                new ProcessMonitor(TimeSpan.FromSeconds(3))
            }
        );

        services.AddSingleton<IDashboardAsyncAuthorizationFilter, HangfireAuthorizationFilter>();

        return services;
    }

    public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
    {
        var serviceProvider = app.ApplicationServices;
        var authorizationFilters = serviceProvider.GetServices<IDashboardAuthorizationFilter>();
        var asyncAuthorizationFilters = serviceProvider.GetServices<IDashboardAsyncAuthorizationFilter>();
        var appSettingRepository = serviceProvider.GetRequiredService<AppSettingRepository>();
        var config = appSettingRepository.GetConfigSection<HangfireConfig>();

        app.UseHangfireDashboard(
            pathMatch: UrlConst.HANGFIRE_URL_PATH,
            options: new DashboardOptions() {
                DashboardTitle = config?.DashboardTitle ?? "Hangfire Dashboard",
                IgnoreAntiforgeryToken = false,
                // Authorization = authorizationFilters,
                // AsyncAuthorization = asyncAuthorizationFilters
            }
        );

        return app;
    }

    private static MongoStorage ToMongoDbStorage(this string connectionString)
    {
        var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
        var settings = MongoClientSettings.FromUrl(mongoUrlBuilder.ToMongoUrl());

        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var mongoClient = new MongoClient(settings);

        mongoClient.GetDatabase(mongoUrlBuilder.DatabaseName);

        var mongoStorage = new MongoStorage(
            mongoClient: mongoClient,
            databaseName: mongoUrlBuilder.DatabaseName,
            storageOptions: new MongoStorageOptions() {
                MigrationOptions = new MongoMigrationOptions {
                    MigrationStrategy = new MigrateMongoMigrationStrategy(),
                    BackupStrategy = new CollectionMongoBackupStrategy()
                },
                CheckConnection = false,
                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.Poll
            }
        );

        return mongoStorage;
    }

    private static IGlobalConfiguration UseMediatR(this IGlobalConfiguration configuration)
    {
        var jsonSettings = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.All
        };

        configuration.UseSerializerSettings(jsonSettings);

        return configuration;
    }
}