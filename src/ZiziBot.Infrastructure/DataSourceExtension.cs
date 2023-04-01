using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Infrastructure;

public static class DataSourceExtension
{
    public static IServiceCollection AddDataSource(this IServiceCollection services)
    {
        var mongodbConnectionString = EnvUtil.GetEnv(Env.MONGODB_CONNECTION_STRING, throwIsMissing: true);

        services.AddTransient<AppSettingsDbContext>(provider => new AppSettingsDbContext(mongodbConnectionString));
        services.AddTransient<AntiSpamDbContext>(provider => new AntiSpamDbContext(mongodbConnectionString));
        services.AddTransient<ChatDbContext>(provider => new ChatDbContext(mongodbConnectionString));
        services.AddTransient<GroupDbContext>(provider => new GroupDbContext(mongodbConnectionString));
        services.AddTransient<MirrorDbContext>(provider => new MirrorDbContext(mongodbConnectionString));
        services.AddTransient<UserDbContext>(provider => new UserDbContext(mongodbConnectionString));

        return services;
    }
}