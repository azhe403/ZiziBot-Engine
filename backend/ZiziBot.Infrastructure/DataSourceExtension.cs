using Microsoft.Extensions.DependencyInjection;
using ZiziBot.DataSource.MongoEf;

namespace ZiziBot.Infrastructure;

public static class DataSourceExtension
{
    public static IServiceCollection AddDataSource(this IServiceCollection services)
    {
        var mongodbConnectionString = EnvUtil.GetEnv(Env.MONGODB_CONNECTION_STRING, throwIsMissing: true);

        services.AddTransient<MongoDbContextBase>(provider => new MongoDbContextBase(mongodbConnectionString));
        services.AddTransient<MongoEfContext>(p => new MongoEfContext());

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