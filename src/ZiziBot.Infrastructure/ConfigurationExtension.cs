using Allowed.Telegram.Bot.Models;
using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoFramework;

namespace ZiziBot.Infrastructure;

public static class ConfigurationExtension
{
    public static IConfigurationBuilder LoadSettings(this IConfigurationBuilder builder)
    {
        DotEnv.Load();

        builder.AddMongoConfigurationSource();

        return builder
            .LoadLocalSettings()
            .LoadAzureAppConfiguration();
    }

    public static IServiceCollection ConfigureSettings(this IServiceCollection services)
    {
        services.AddDataSource();

        var provider = services.BuildServiceProvider();

        var config = provider.GetRequiredService<IConfiguration>();
        var appSettingDbContext = provider.GetRequiredService<AppSettingsDbContext>();

        services.Configure<CacheConfig>(config.GetSection("Cache"));
        services.Configure<EventLogConfig>(config.GetSection("EventLog"));
        services.Configure<HangfireConfig>(config.GetSection("Hangfire"));
        services.Configure<List<SimpleTelegramBotClientOptions>>(
            list => {
                var host = EnvUtil.GetEnv(Env.TELEGRAM_WEBHOOK_URL);
                var listBotData = appSettingDbContext.BotSettings
                    .AsEnumerable()
                    .Select(settings => new SimpleTelegramBotClientOptions(settings.Name, settings.Token, host, null, false))
                    .ToList();

                list.AddRange(listBotData);
            }
        );

        if (EnvUtil.IsEnvExist(Env.AZURE_APP_CONFIG_CONNECTION_STRING))
            services.AddAzureAppConfiguration();

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
            .Where(file => !file.EndsWith("x.json"))// End with x.json to ignore
            .ToList();

        settingFiles.ForEach(file => builder.AddJsonFile(file, reloadOnChange: true, optional: false));

        return builder;
    }

    private static IConfigurationBuilder LoadAzureAppConfiguration(this IConfigurationBuilder builder)
    {
        var connectionString = EnvUtil.GetEnv(Env.AZURE_APP_CONFIG_CONNECTION_STRING);

        if (string.IsNullOrEmpty(connectionString)) return builder;

        builder.AddAzureAppConfiguration(options => { options.Connect(connectionString); });

        return builder;
    }

    private static IConfigurationBuilder AddMongoConfigurationSource(this IConfigurationBuilder builder)
    {
        var mongodbConnectionString = EnvUtil.GetEnv(Env.MONGODB_CONNECTION_STRING);

        var mongoDbConnection = MongoDbConnection.FromConnectionString(mongodbConnectionString);
        if (string.IsNullOrEmpty(mongoDbConnection.Url.DatabaseName))
        {
            throw new Exception("Database name is not specified in connection string. Example: mongodb://localhost:27017/DatabaseName");
        }

        builder.Add(new MongoConfigSource(mongodbConnectionString));

        return builder;
    }
}