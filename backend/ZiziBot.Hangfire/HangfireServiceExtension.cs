using Hangfire.LiteDB;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Sentry.Hangfire;
using Serilog;
using ZiziBot.Common.Exceptions;

namespace ZiziBot.Hangfire;

public static class HangfireServiceExtension
{
    public static IServiceCollection ConfigureHangfire(this IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Hangfire");
        var hangfireConfig = serviceProvider.GetRequiredService<IOptionsSnapshot<HangfireConfig>>().Value;

        if (EnvUtil.IsEnabled(Flag.HANGFIRE))
        {

            JobStorage jobStorage = hangfireConfig.CurrentStorage switch {
                CurrentStorage.MongoDb => hangfireConfig.MongoDbConnection.ToMongoDbStorage(),
                CurrentStorage.Sqlite => new SQLiteStorage(PathConst.HANGFIRE_SQLITE_PATH.EnsureDirectory()),
                CurrentStorage.LiteDb => new LiteDbStorage(PathConst.HANGFIRE_LITEDB_PATH.EnsureDirectory()),
                _ => new InMemoryStorage(new InMemoryStorageOptions())
            };

            services.AddHangfire(configuration => {
                    configuration
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseDarkDashboard()
                        .UseStorage(jobStorage)
                        .UseHeartbeatPage(checkInterval: TimeSpan.FromSeconds(3))
                        .UseMediatR()
                        .UseSentry()
                        .UseSerilogLogProvider();
                }
            );

            if (EnvUtil.IsEnabled(Flag.HANGFIRE_SEPARATED_SERVER))
            {
                logger.LogDebug("Hangfire is running in a separated server!");
                var queues = hangfireConfig.Queues;

                foreach (var queue in queues)
                {
                    services.AddHangfireServer(
                        optionsAction: (provider, options) => {
                            options.WorkerCount = queue.WorkerCount;
                            options.Queues = [queue.Name];
                        },
                        storage: jobStorage,
                        additionalProcesses: [
                            new ProcessMonitor(TimeSpan.FromSeconds(3))
                        ]
                    );
                }
            }
            else
            {
                logger.LogDebug("Hangfire is running in a single server!");
                var queues = hangfireConfig.Queues.Select(x => x.Name).ToArray();

                services.AddHangfireServer(
                    optionsAction: (provider, options) => {
                        options.WorkerCount = Environment.ProcessorCount * hangfireConfig.WorkerMultiplier;
                        options.Queues = queues;
                    },
                    storage: jobStorage,
                    additionalProcesses: [
                        new ProcessMonitor(TimeSpan.FromSeconds(3))
                    ]
                );
            }

            services.AddScoped<IDashboardAsyncAuthorizationFilter, HangfireAuthorizationFilter>();
        }
        else
        {
            logger.LogDebug("Hangfire is disabled!");
        }

        return services;
    }

    public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
    {
        var serviceProvider = app.ApplicationServices;
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Hangfire");

        if (EnvUtil.IsEnabled(Flag.HANGFIRE))
        {
            var scope = serviceProvider.CreateScope();
            var serviceScope = scope.ServiceProvider;
            var appSettingRepository = serviceScope.GetRequiredService<AppSettingRepository>();
            var config = appSettingRepository.GetConfigSection<HangfireConfig>();

            var dashboardOptions = new DashboardOptions() {
                DashboardTitle = config?.DashboardTitle ?? "Hangfire Dashboard",
                IgnoreAntiforgeryToken = false,
                AppPath = EnvUtil.GetEnv(Env.WEB_CONSOLE_URL)
            };

            if (EnvUtil.IsEnabled(Flag.HANGFIRE_ENABLE_AUTH))
            {
                var authorizationFilters = serviceScope.GetServices<IDashboardAuthorizationFilter>();
                var asyncAuthorizationFilters = serviceScope.GetServices<IDashboardAsyncAuthorizationFilter>();

                dashboardOptions.Authorization = authorizationFilters;
                dashboardOptions.AsyncAuthorization = asyncAuthorizationFilters;
            }

            app.UseHangfireDashboard(
                pathMatch: UrlConst.HANGFIRE_URL_PATH,
                options: dashboardOptions
            );
        }
        else
        {
            logger.LogInformation("Hangfire is disabled!");
        }

        return app;
    }

    private static MongoStorage ToMongoDbStorage(this string? connectionString)
    {
        var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
        var settings = MongoClientSettings.FromUrl(mongoUrlBuilder.ToMongoUrl());

        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var mongoClient = new MongoClient(settings);

        var databaseName = mongoUrlBuilder.DatabaseName;

        mongoClient.GetDatabase(databaseName);

        try
        {
            var mongoStorage = new MongoStorage(
                mongoClient: mongoClient,
                databaseName: databaseName,
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
        catch (Exception ex)
        {
            Log.Error(ex, "Fail to create Hangfire MongoDB Storage. Try to reset the database.");

            mongoClient.GetDatabase(databaseName)
                .ListCollectionNames()
                .ToList()
                .Where(x => x.StartsWith("hangfire", StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(x => mongoClient.GetDatabase(databaseName).DropCollection(x));

            throw new AppException("Fail to create Hangfire MongoDB Storage. Please restart Engine");
        }
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