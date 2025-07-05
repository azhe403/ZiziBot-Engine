using CacheTower;
using CacheTower.Serializers.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ZiziBot.Caching.Json;
using ZiziBot.Caching.MongoDb;
using ZiziBot.Caching.Redis;

namespace ZiziBot.Caching.Extensions;

public static class CacheTowerExtension
{
    private static ICacheSerializer CurrentSerializer => new SystemTextJsonCacheSerializer(new() {
        WriteIndented = true
    });

    public static IServiceCollection AddCacheTower(this IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        var cacheConfig = serviceProvider.GetRequiredService<IOptions<CacheConfig>>().Value;
        var gcpConfig = serviceProvider.GetRequiredService<IOptions<GcpConfig>>().Value;

        var firebaseConfig = new FirebaseCacheOptions() {
            ProjectUrl = gcpConfig.FirebaseProjectUrl,
            ServiceAccountJson = gcpConfig.FirebaseServiceAccountJson,
            RootDir = cacheConfig.PrefixRoot
        };

        services.AddCacheStack(builder => {
            builder
                .WithCleanupFrequency(TimeSpan.FromMinutes(10))
                .AddMemoryCacheLayer()
                .ConfigureFileCacheLayer(cacheConfig)
                .ConfigureSqliteCacheLayer(cacheConfig)
                .ConfigureMongoDbCacheLayer(cacheConfig)
                .ConfigureRedisCacheLayer(cacheConfig)
                .ConfigureFirebaseCacheLayer(firebaseConfig: firebaseConfig);
        });

        return services;
    }

    private static ICacheStackBuilder ConfigureRedisCacheLayer(this ICacheStackBuilder builder, CacheConfig cacheConfig)
    {
        if (!cacheConfig.UseRedis)
            return builder;

        if (string.IsNullOrEmpty(cacheConfig.RedisConnection))
            return builder;

        builder.CacheLayers.Add(new RedisLayerProvider(
            cacheConfig.RedisConnection,
            new(
                CurrentSerializer,
                PrefixRoot: ValueConst.UniqueKey
            )
        ));

        return builder;
    }

    private static ICacheStackBuilder ConfigureFileCacheLayer(this ICacheStackBuilder builder, CacheConfig cacheConfig)
    {
        if (!cacheConfig.UseJsonFile)
            return builder;

        builder.CacheLayers.Add(
            new JsonLayerProvider() {
                DirPath = PathConst.CACHE_TOWER_JSON.EnsureDirectory(),
                Serializer = CurrentSerializer
            });

        return builder;
    }

    private static ICacheStackBuilder ConfigureFirebaseCacheLayer(
        this ICacheStackBuilder builder,
        CacheConfig? cacheConfig = null,
        FirebaseCacheOptions? firebaseConfig = null
    )
    {
        if (cacheConfig?.UseFirebase == false)
            return builder;

        FirebaseCacheOptions firebaseOptions;

        if (firebaseConfig != null)
        {
            firebaseOptions = firebaseConfig;
        }
        else if (cacheConfig != null)
        {
            firebaseOptions = new FirebaseCacheOptions {
                ProjectUrl = cacheConfig.FirebaseProjectUrl,
                ServiceAccountJson = cacheConfig.FirebaseServiceAccountJson
            };
        }
        else
        {
            return builder;
        }

        builder.CacheLayers.Add(new FirebaseLayerProvider(firebaseOptions));

        return builder;
    }

    private static ICacheStackBuilder ConfigureSqliteCacheLayer(
        this ICacheStackBuilder builder,
        CacheConfig cacheConfig
    )
    {
        if (!cacheConfig.UseSqlite)
            return builder;

        var dbPath = PathConst.CACHE_TOWER_SQLITE_PATH.EnsureDirectory();

        builder.CacheLayers.Add(new SqliteLayerProvider(dbPath));

        return builder;
    }

    private static ICacheStackBuilder ConfigureMongoDbCacheLayer(
        this ICacheStackBuilder builder,
        CacheConfig cacheConfig
    )
    {
        if (!cacheConfig.UseMongoDb)
            return builder;

        var dbPath = EnvUtil.GetEnv(Env.MONGODB_CONNECTION_STRING);

        builder.CacheLayers.Add(new MongoLayerProvider(new() {
            ConnectionString = dbPath,
            Serializer = CurrentSerializer
        }));

        return builder;
    }
}