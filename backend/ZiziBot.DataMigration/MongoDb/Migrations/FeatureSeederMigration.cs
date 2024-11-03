using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ZiziBot.Contracts.Constants;
using ZiziBot.Contracts.Enums;
using ZiziBot.DataMigration.MongoDb.Interfaces;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.DataMigration.MongoDb.Migrations;

public class FeatureSeederMigration(MongoEfContext mongoEfContext) : IPreMigration
{
    public string Id { get; }

    public async Task UpAsync(IMongoDatabase database)
    {
        var transactionId = Guid.NewGuid().ToString();
        var listFlags = Flag.GetFields();

        foreach (var flag in listFlags)
        {
            var featureFlagEntity = await mongoEfContext.FeatureFlag
                .Where(x => x.Name == flag.Name)
                .Where(x => x.Status == EventStatus.Complete)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            if (featureFlagEntity == null)
            {
                mongoEfContext.FeatureFlag.Add(new FeatureFlagEntity {
                    Name = flag.Name,
                    IsEnabled = flag.Value,
                    Status = EventStatus.Complete,
                    TransactionId = transactionId
                });
            }
        }

        await mongoEfContext.SaveChangesAsync();
    }

    public async Task DownAsync(IMongoDatabase database)
    { }
}