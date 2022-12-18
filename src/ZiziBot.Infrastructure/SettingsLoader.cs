using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Infrastructure;

public static class SettingsLoader
{
    public static IConfigurationBuilder LoadSettings(this IConfigurationBuilder builder)
    {
        return builder
            .LoadLocalSettings()
            .LoadAzureAppConfiguration();
    }

    public static IServiceCollection ConfigureSettings(this IServiceCollection services)
    {
        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        services.Configure<HangfireConfig>(config.GetSection("Hangfire"));

        services.AddAzureAppConfiguration();

        return services;
    }

    private static IConfigurationBuilder LoadLocalSettings(this IConfigurationBuilder builder)
    {
        var settingsPath = Path.Combine(Environment.CurrentDirectory, "Storage", "AppSettings", "Current");
        var settingFiles = Directory.GetFiles(settingsPath)
            .Where(file => !file.EndsWith("x.json")) // End with x.json to ignore
            .ToList();

        settingFiles.ForEach(file => builder.AddJsonFile(file, reloadOnChange: true, optional: false));

        return builder;
    }

    private static IConfigurationBuilder LoadAzureAppConfiguration(this IConfigurationBuilder builder)
    {
        var connectionString = Environment.GetEnvironmentVariable("AZURE_APP_CONFIG_CONNECTION_STRING");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ApplicationException("AZURE_APP_CONFIG_CONNECTION_STRING is not set");
        }

        builder.AddAzureAppConfiguration(options => { options.Connect(connectionString); });

        return builder;
    }
}