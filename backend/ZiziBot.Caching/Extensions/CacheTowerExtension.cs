using CacheTower.Providers.FileSystem;
using CacheTower.Providers.Redis;
using CacheTower.Serializers.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ZiziBot.Caching.Extensions;

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
                    .WithCleanupFrequency(TimeSpan.FromDays(1))
                    .AddMemoryCacheLayer()
                    .ConfigureRedisCacheLayer(cacheConfig)
                    .ConfigureFileCacheLayer(cacheConfig)
                    .ConfigureFirebaseCacheLayer(cacheConfig);
            }
        );

        return services;
    }

    private static ICacheStackBuilder ConfigureRedisCacheLayer(this ICacheStackBuilder builder, CacheConfig cacheConfig)
    {
        if (!cacheConfig.UseRedis)
            return builder;

        if (string.IsNullOrEmpty(cacheConfig.RedisConnection))
            return builder;

        builder.AddRedisCacheLayer(
            connection: ConnectionMultiplexer.Connect(cacheConfig.RedisConnection),
            options: new RedisCacheLayerOptions(
                new NewtonsoftJsonCacheSerializer(new JsonSerializerSettings() { Formatting = Formatting.Indented })
            )
        );

        return builder;
    }

    private static ICacheStackBuilder ConfigureFileCacheLayer(this ICacheStackBuilder builder, CacheConfig cacheConfig)
    {
        if (!cacheConfig.UseJsonFile)
            return builder;

        builder.AddFileCacheLayer(
            new FileCacheLayerOptions(
                DirectoryPath: PathConst.CACHE_TOWER_PATH.EnsureDirectory(),
                Serializer: new NewtonsoftJsonCacheSerializer(new JsonSerializerSettings() { Formatting = Formatting.Indented })
            )
        );

        return builder;
    }

    private static ICacheStackBuilder ConfigureFirebaseCacheLayer(this ICacheStackBuilder builder, CacheConfig cacheConfig)
    {
        if (!cacheConfig.UseFirebase) return builder;

        var firebaseOptions = new FirebaseCacheOptions
        {
            ProjectUrl = cacheConfig.FirebaseProjectUrl,
            ServiceAccountJson = cacheConfig.FirebaseServiceAccountJson
        };

        builder.CacheLayers.Add(new CacheTowerFirebaseProvider(firebaseOptions));

        return builder;
    }
}