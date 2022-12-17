using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Infrastructure;

public static class CacheTowerExtension
{
    public static IServiceCollection AddCacheTower(this IServiceCollection services)
    {
        var firebaseOptions = new FirebaseCacheOptions
        {
            ProjectUrl = "",
            ServiceAccountJson = @""
        };

        services.AddCacheStack(builder =>
        {
            builder.CacheLayers.Add(new CacheTowerFirebaseProvider(firebaseOptions));
        });

        return services;
    }
}