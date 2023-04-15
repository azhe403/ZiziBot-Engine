using CloudCraic.Hosting.BackgroundQueue.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ZiziBot.Infrastructure;

public static class ServiceExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureSettings();
        services.AddKotMongoMigrations();

        services.AddMediator();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddBackgroundQueue();
        services.AddCacheTower();
        services.AddAllService();
        services.AddAllMiddleware();

        return services;
    }

    private static IServiceCollection AddAllService(this IServiceCollection services)
    {
        services.Scan(selector =>
            selector.FromAssembliesOf(typeof(CacheService))
                .AddClasses(filter => filter.InNamespaceOf<CacheService>())
                .AsSelf()
                .WithTransientLifetime()
        );

        services.Scan(selector =>
            selector.FromAssembliesOf(typeof(RegisterRssJobTasks))
                .AddClasses(filter => filter.InNamespaceOf<RegisterRssJobTasks>())
                .As<IStartupTask>()
                .WithTransientLifetime()
        );

        services.Scan(selector =>
            selector.FromAssembliesOf(typeof(TaskRunnerHostedService))
                .AddClasses(filter => filter.InNamespaceOf<TaskRunnerHostedService>())
                .As<IHostedService>()
                .WithTransientLifetime()
        );

        services.Scan(selector =>
            selector.FromAssembliesOf(typeof(GithubWebhookEventHandler))
                .AddClasses(filter => filter.InNamespaceOf<GithubWebhookEventHandler>())
                .As<GithubWebhookEventProcessor>()
                .WithTransientLifetime()
        );

        return services;
    }

    private static IServiceCollection AddBackgroundQueue(this IServiceCollection services)
    {
        services.AddBackgroundQueue(
            maxConcurrentCount: 3,
            millisecondsToWaitBeforePickingUpTask: 1000,
            onException: exception => { }
        );

        return services;
    }
}