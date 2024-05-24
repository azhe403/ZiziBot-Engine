using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Tasks;

public class SeedFlagTask(
    ILogger<SeedFlagTask> logger,
    MongoDbContextBase mongoDbContextBase
) : IStartupTask
{
    public bool SkipAwait { get; set; }

    public async Task ExecuteAsync()
    {
        logger.LogInformation("");

        var transactionId = Guid.NewGuid().ToString();
        var listFlags = Flag.GetFields();

        foreach (var flag in listFlags)
        {
            var featureFlagEntity = await mongoDbContextBase.FeatureFlag
                .Where(x => x.Name == flag.Name)
                .Where(x => x.Status == (int)EventStatus.Complete)
                .FirstOrDefaultAsync();

            if (featureFlagEntity == null)
            {
                mongoDbContextBase.FeatureFlag.Add(new FeatureFlagEntity() {
                    Name = flag.Name,
                    IsEnabled = flag.Value,
                    Status = (int)EventStatus.Complete,
                    TransactionId = transactionId
                });
            }
        }

        await mongoDbContextBase.SaveChangesAsync();
    }
}