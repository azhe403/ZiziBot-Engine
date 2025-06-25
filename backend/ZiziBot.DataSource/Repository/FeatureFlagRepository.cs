using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ZiziBot.Common.Dtos.Entity;
using ZiziBot.Common.Interfaces;
using ZiziBot.Common.Utils;
using ZiziBot.DataSource.MongoEf;

namespace ZiziBot.DataSource.Repository;

public class FeatureFlagRepository(MongoEfContext mongoEfContext, IServiceProvider serviceProvider)
{
    private ICacheService cacheService => serviceProvider.GetRequiredService<ICacheService>();

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

    public async Task<FeatureFlagDto?> GetFlag(string flagName)
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

                if (flag == null)
                    return null;

                return new FeatureFlagDto() {
                    Id = flag.Id.ToString(),
                    Name = flag.Name,
                    IsEnabled = flag.IsEnabled,
                    Status = (int)flag.Status,
                    CreatedDate = flag.CreatedDate,
                    CreatedBy = flag.CreatedBy,
                    UpdatedDate = flag.CreatedDate,
                    UpdatedBy = flag.UpdatedBy,
                    TransactionId = flag.TransactionId
                };
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