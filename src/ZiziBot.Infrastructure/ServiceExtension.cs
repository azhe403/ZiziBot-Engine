using System.Reflection;
using CloudCraic.Hosting.BackgroundQueue.DependencyInjection;
using FluentValidation;
using MediatR;
using MediatR.Extensions.AttributedBehaviors;
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

        services.AddBackgroundQueue();
        services.AddCacheTower();
        services.AddAllService();
        services.AddAllMiddleware();

        services.AddValidatorsFromAssemblyContaining<PostGlobalBanApiValidator>();

        return services;
    }

    private static IServiceCollection AddMediator(this IServiceCollection services)
    {
        var assembly = typeof(PingRequestHandler).GetTypeInfo().Assembly;

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<PingRequestHandler>());

        services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(GlobalExceptionHandler<,,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AntiSpamPipelineBehaviour<,>));

        services.AddMediatRAttributedBehaviors(assembly);

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

    private static IServiceCollection AddBackgroundQueue(this IServiceCollection services)
    {
        services.AddBackgroundQueue(
            maxConcurrentCount: 3,
            millisecondsToWaitBeforePickingUpTask: 1000,
            onException: exception => {

            }
        );

        return services;
    }
}