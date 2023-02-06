using CacheTower.Providers.FileSystem;
using CacheTower.Serializers.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ZiziBot.Infrastructure;

public static class CacheTowerExtension
{
    public static IServiceCollection AddCacheTower(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var cacheConfig = serviceProvider.GetRequiredService<IOptions<CacheConfig>>().Value;

        // var firebaseOptions = new FirebaseCacheOptions
        // {
        //     ProjectUrl = cacheConfig.FirebaseProjectUrl,
        //     ServiceAccountJson = cacheConfig.FirebaseServiceAccountJson
        // };

        services.AddCacheStack(
            builder => {
                // builder.CacheLayers.Add(new CacheTowerFirebaseProvider(firebaseOptions));
                builder.AddFileCacheLayer(new FileCacheLayerOptions("Storage/CacheTower/File/".EnsureDirectory(), NewtonsoftJsonCacheSerializer.Instance))
                    .WithCleanupFrequency(TimeSpan.FromDays(1));
            }
        );

        return services;
    }
}