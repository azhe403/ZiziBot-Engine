using CacheTower.Providers.FileSystem;
using CacheTower.Serializers.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ZiziBot.Infrastructure;

public static class CacheTowerExtension
{
    public static IServiceCollection AddCacheTower(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var cacheConfig = serviceProvider.GetRequiredService<IOptions<CacheConfig>>().Value;

        services.AddCacheStack(
            builder => {
                builder
                    .AddMemoryCacheLayer()
                    .AddFileCacheLayer(
                        new FileCacheLayerOptions(
                            directoryPath: PathConst.CACHE_TOWER_PATH,
                            serializer: new NewtonsoftJsonCacheSerializer(
                                new JsonSerializerSettings()
                                {
                                    Formatting = environment.IsDevelopment() ? Formatting.Indented : Formatting.None
                                }
                            ),
                            manifestSaveInterval: TimeSpan.FromSeconds(20)
                        )
                    )
                    .AddFirebaseCacheLayer(cacheConfig)
                    .WithCleanupFrequency(TimeSpan.FromDays(1));
            }
        );

        return services;
    }

    private static ICacheStackBuilder AddFirebaseCacheLayer(this ICacheStackBuilder builder, CacheConfig cacheConfig)
    {
        var firebaseOptions = new FirebaseCacheOptions
        {
            ProjectUrl = cacheConfig.FirebaseProjectUrl,
            ServiceAccountJson = cacheConfig.FirebaseServiceAccountJson
        };

        builder.CacheLayers.Add(new CacheTowerFirebaseProvider(firebaseOptions));

        return builder;
    }
}