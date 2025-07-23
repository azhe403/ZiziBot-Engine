using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZiziBot.Common.Exceptions;

namespace ZiziBot.Infrastructure;

public static class ConfigurationExtension
{
    public static IConfigurationBuilder LoadSettings(this IConfigurationBuilder builder)
    {
        DotEnv.Load();

        builder.AddMongoConfigurationSource();

        return builder.LoadLocalSettings();
    }

    public static async Task<IServiceCollection> ConfigureSettings(this IServiceCollection services)
    {
        services.AddDataSource();
        services.AddDataRepository();

        await using var provider = services.BuildServiceProvider();

        var config = provider.GetRequiredService<IConfiguration>();

        services.Configure<CacheConfig>(config.GetSection("Cache"));
        services.Configure<EngineConfig>(config.GetSection("Engine"));
        services.Configure<EventLogConfig>(config.GetSection("EventLog"));
        services.Configure<GcpConfig>(config.GetSection("Gcp"));
        services.Configure<HangfireConfig>(config.GetSection("Hangfire"));
        services.Configure<JwtConfig>(config.GetSection("Jwt"));
        services.Configure<OptiicDevConfig>(config.GetSection("OptiicDev"));

        return services;
    }

    private static IConfigurationBuilder LoadLocalSettings(this IConfigurationBuilder builder)
    {
        var settingsPath = Path.Combine(Environment.CurrentDirectory, "Storage", "AppSettings", "Current");

        if (!Directory.Exists(settingsPath))
        {
            return builder;
        }

        var settingFiles = Directory.GetFiles(settingsPath)
            .Where(file => !file.EndsWith("x.json")) // End with x.json to ignore
            .ToList();

        settingFiles.ForEach(file => builder.AddJsonFile(file, reloadOnChange: true, optional: false));

        return builder;
    }

    internal static async Task<IServiceCollection> PrefetchRepository(this IServiceCollection app)
    {
        await using var provider = app.BuildServiceProvider();

        #region Feature Flags
        var featureFlagRepository = provider.GetRequiredService<FeatureFlagRepository>();
        await featureFlagRepository.GetFlags();
        #endregion

        #region Env
        var appSettingRepository = provider.GetRequiredService<AppSettingRepository>();
        var sentryConfig = await appSettingRepository.GetConfigSectionAsync<SentryConfig>();

        if (sentryConfig?.IsEnabled == true)
        {
            Env.SentryDsn = sentryConfig.Dsn;
        }
        #endregion

        return app;
    }

    private static IConfigurationBuilder AddMongoConfigurationSource(this IConfigurationBuilder builder)
    {
        var mongodbConnectionString = EnvUtil.GetEnv(Env.MONGODB_CONNECTION_STRING, throwIsMissing: true);
        var url = mongodbConnectionString.ToMongoUrl();

        if (url.DatabaseName.IsNullOrEmpty())
        {
            throw new AppException("Database name is not specified in Connection String. Example: mongodb://localhost:27017/DatabaseName");
        }

        builder.Add(new MongoConfigSource(mongodbConnectionString));

        return builder;
    }
}