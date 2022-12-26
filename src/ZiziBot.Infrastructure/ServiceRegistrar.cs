using System.Reflection;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Infrastructure;

public static class ServiceRegistrar
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureSettings();

        services.AddMediatR(typeof(PingRequestHandler).GetTypeInfo().Assembly);
        services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(ExceptionHandler<,,>));

        services.AddCacheTower();
        services.AddServices();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        // services.AddScoped<FirebaseService>();
        // services.AddScoped<CacheService>();

        services.Scan(scan => scan
            .FromAssembliesOf(typeof(PingRequestHandler))
            .AddClasses(filter => filter.InNamespaceOf<CacheService>())
            .AsSelf()
            .WithScopedLifetime());

        return services;
    }
}