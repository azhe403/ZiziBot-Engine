using Microsoft.Extensions.DependencyInjection;
using ZiziBot.Database.MongoDb;
using ZiziBot.Database.Service;

namespace ZiziBot.Infrastructure;

public static class DataSourceExtension
{
    public static IServiceCollection AddDataSource(this IServiceCollection services)
    {
        services.AddTransient<MongoDbContext>();

        services.Scan(selector => selector.FromAssembliesOf(typeof(CacheService))
            .AddClasses(filter => filter.InNamespaceOf<CacheService>())
            .AsSelfWithInterfaces()
            .WithTransientLifetime());

        return services;
    }

    public static IServiceCollection AddDataRepository(this IServiceCollection services)
    {
        services.Scan(selector => {
            selector.FromAssembliesOf(typeof(AppSettingRepository))
                .AddClasses(filter => filter.InNamespaceOf(typeof(AppSettingRepository)))
                .AsSelf()
                .WithTransientLifetime();
        });

        return services;
    }
}