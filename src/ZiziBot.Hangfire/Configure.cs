using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ZiziBot.Hangfire;

public static class Configure
{
    public static IServiceCollection ConfigureHangfire(this IServiceCollection services)
    {
        JobStorage.Current = new InMemoryStorage();

        services.AddHangfire(configuration =>
        {
            configuration
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings();

            // configuration.UseInMemoryStorage();
            configuration.UseDarkDashboard();
            configuration.UseMediatR();

            configuration.UseHeartbeatPage(checkInterval: TimeSpan.FromSeconds(3));
        });

        services.AddHangfireServer(storage: JobStorage.Current, additionalProcesses: new[] { new ProcessMonitor(checkInterval: TimeSpan.FromSeconds(1)) });

        return services;
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