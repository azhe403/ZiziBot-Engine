using System.Reflection;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Infrastructure;

public static class ServiceExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureSettings();

        services.AddMediator();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddCacheTower();
        services.AddAllService();
        services.AddAllMiddleware();

        return services;
    }

    private static IServiceCollection AddMediator(this IServiceCollection services)
    {
        var assembly = typeof(PingRequestHandler).GetTypeInfo().Assembly;

        services.AddMediatR(assembly);
        services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(GlobalExceptionHandler<,,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));

        return services;
    }

    private static IServiceCollection AddAllService(this IServiceCollection services)
    {
        services.Scan(
            selector =>
                selector.FromAssembliesOf(typeof(CacheService))
                    .AddClasses(filter => filter.InNamespaceOf<CacheService>())
                    .AsSelf()
                    .WithTransientLifetime()
        );

        return services;
    }
}