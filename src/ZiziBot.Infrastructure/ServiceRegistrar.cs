using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Infrastructure;

public static class ServiceRegistrar
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureSettings();

        services.AddCacheTower();
        services.AddServices();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<FirebaseService>();
        services.AddScoped<CacheService>();

        return services;
    }
}