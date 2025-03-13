using Microsoft.EntityFrameworkCore;
using Serilog;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.DataSource.Repository;

public class FeatureFlagRepository(MongoEfContext mongoEfContext, ICacheService cacheService)
{
    public async Task<List<FlagDto>?> GetFlags()
    {
        var flags = await mongoEfContext.FeatureFlag.AsNoTracking()
            .Where(x => x.Status == EventStatus.Complete)
            .Select(x => new FlagDto() {
                Name = x.Name,
                Value = x.IsEnabled
            })
            .ToListAsync();

        return flags;
    }

    public async Task<FeatureFlagEntity?> GetFlag(string flagName)
    {
        if (flagName.IsNullOrWhiteSpace())
            return null;

        var cache = await cacheService.GetOrSetAsync(
            cacheKey: CacheKey.FEATURE_FLAG + flagName,
            action: async () => {
                var flag = await mongoEfContext.FeatureFlag.AsNoTracking()
                    .Where(x => x.Name == flagName)
                    .Where(x => x.Status == EventStatus.Complete)
                    .FirstOrDefaultAsync();

                return flag;
            }
        );

        return cache;
    }

    public async Task<bool> GetFlagValue(string flagName)
    {
        var flag = await GetFlag(flagName);

        var isEnabled = (bool)flag?.IsEnabled;
        Log.Debug("Flag: '{flagName}' is enabled: {isEnabled}", flagName, isEnabled);

        return isEnabled;
    }
}