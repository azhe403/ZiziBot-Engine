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

        logger.LogInformation("Configuring Hangfire. Storage type: {StorageType}", hangfireConfig.CurrentStorage);

        JobStorage.Current = hangfireConfig.CurrentStorage switch
        {
            CurrentStorage.MongoDb => hangfireConfig.MongoDbConnection.ToMongoDbStorage(),
            _ => new InMemoryStorage(new InMemoryStorageOptions())
        };

        services.AddHangfire(configuration =>
        {
            configuration
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseDarkDashboard()
                .UseMediatR();

            configuration.UseHeartbeatPage(checkInterval: TimeSpan.FromSeconds(3));
        });

        services.AddHangfireServer(storage: JobStorage.Current, additionalProcesses: new[] { new ProcessMonitor(checkInterval: TimeSpan.FromSeconds(1)) });

        return services;
    }

    public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard(options: new DashboardOptions()
        {
            DashboardTitle = "Zizi Dev - Hangfire Dashboard",
            IgnoreAntiforgeryToken = true,
            Authorization = new[] { new HangfireAuthorization() }
        });

        return app;
    }

    private static MongoStorage ToMongoDbStorage(this string connectionString)
    {
        var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
        var settings = MongoClientSettings.FromUrl(mongoUrlBuilder.ToMongoUrl());

        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var mongoClient = new MongoClient(settings);

        mongoClient.GetDatabase(mongoUrlBuilder.DatabaseName);

        var mongoStorage = new MongoStorage(mongoClient, mongoUrlBuilder.DatabaseName,
            new MongoStorageOptions()
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new MigrateMongoMigrationStrategy(),
                    BackupStrategy = new CollectionMongoBackupStrategy()
                },
                CheckConnection = false,
                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.Poll
            });

        return mongoStorage;
    }

    private static void UseMediatR(this IGlobalConfiguration configuration)
    {
        var jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        configuration.UseSerializerSettings(jsonSettings);
    }
}