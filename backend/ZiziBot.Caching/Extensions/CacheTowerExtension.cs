using System.Text.Json;
using CacheTower.Providers.Database.MongoDB;
using CacheTower.Providers.FileSystem;
using CacheTower.Providers.Redis;
using CacheTower.Serializers.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoFramework;
using ZiziBot.Caching.Redis;

namespace ZiziBot.Caching.Extensions;

public static class CacheTowerExtension
{
    public static IServiceCollection AddCacheTower(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var cacheConfig = serviceProvider.GetRequiredService<IOptions<CacheConfig>>().Value;

        services.AddCacheStack(
            builder => {
                builder
                    .WithCleanupFrequency(TimeSpan.FromDays(1))
                    .AddMemoryCacheLayer()
                    .ConfigureFileCacheLayer(cacheConfig)
                    .ConfigureSqliteCacheLayer(cacheConfig)
                    .ConfigureMongoDbCacheLayer(cacheConfig)
                    .ConfigureRedisCacheLayer(cacheConfig)
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

        builder.CacheLayers.Add(new CacheTowerRedisProvider(
            connectionString: cacheConfig.RedisConnection,
            options: new RedisCacheLayerOptions(
                new SystemTextJsonCacheSerializer(new JsonSerializerOptions() {
                    WriteIndented = true
                })
            )
        ));

        return builder;
    }

    private static ICacheStackBuilder ConfigureFileCacheLayer(this ICacheStackBuilder builder, CacheConfig cacheConfig)
    {
        if (!cacheConfig.UseJsonFile)
            return builder;

        builder.AddFileCacheLayer(
            new FileCacheLayerOptions(
                DirectoryPath: PathConst.CACHE_TOWER_PATH.EnsureDirectory(),
                Serializer: new SystemTextJsonCacheSerializer(new JsonSerializerOptions() {
                    WriteIndented = true
                })
            )
        );

        return builder;
    }

    private static ICacheStackBuilder ConfigureFirebaseCacheLayer(this ICacheStackBuilder builder,
        CacheConfig cacheConfig)
    {
        if (!cacheConfig.UseFirebase)
            return builder;

        var firebaseOptions = new FirebaseCacheOptions {
            ProjectUrl = cacheConfig.FirebaseProjectUrl,
            ServiceAccountJson = cacheConfig.FirebaseServiceAccountJson
        };

        builder.CacheLayers.Add(new CacheTowerFirebaseProvider(firebaseOptions));

        return builder;
    }

    private static ICacheStackBuilder ConfigureSqliteCacheLayer(this ICacheStackBuilder builder,
        CacheConfig cacheConfig)
    {
        if (!cacheConfig.UseSqlite)
            return builder;

        var dbPath = PathConst.CACHE_TOWER_SQLITE_PATH.EnsureDirectory();

        builder.CacheLayers.Add(new CacheTowerSqliteProvider(dbPath));

        return builder;
    }

    private static ICacheStackBuilder ConfigureMongoDbCacheLayer(this ICacheStackBuilder builder,
        CacheConfig cacheConfig)
    {
        if (!cacheConfig.UseMongoDb)
            return builder;

        var dbPath = EnvUtil.GetEnv(Env.MONGODB_CONNECTION_STRING);

        builder.CacheLayers.Add(new MongoDbCacheLayer(MongoDbConnection.FromConnectionString(dbPath)));

        return builder;
    }
}