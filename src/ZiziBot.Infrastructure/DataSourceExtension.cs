using Microsoft.Extensions.DependencyInjection;
using MongoFramework;

namespace ZiziBot.Infrastructure;

public static class DataSourceExtension
{
    public static IServiceCollection AddDataSource(this IServiceCollection services)
    {
        var mongodbConnectionString = EnvUtil.GetEnv(Env.MONGODB_CONNECTION_STRING);

        if (mongodbConnectionString == null)
        {
            throw new ApplicationException($"{Env.MONGODB_CONNECTION_STRING} environment variable is not set.");
        }

        services.AddScoped<AppSettingsDbContext>(provider => new AppSettingsDbContext(mongodbConnectionString));
        services.AddScoped<UserDbContext>(provider => new UserDbContext(mongodbConnectionString));

        return services;
    }
}