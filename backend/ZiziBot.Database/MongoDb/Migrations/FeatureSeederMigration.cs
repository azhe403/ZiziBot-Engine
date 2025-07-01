﻿using Microsoft.EntityFrameworkCore;
using ZiziBot.Database.MongoDb.Entities;
using ZiziBot.Database.MongoDb.Interfaces;

namespace ZiziBot.Database.MongoDb.Migrations;

public class FeatureSeederMigration(MongoDbContext mongoDbContext) : IPreMigration
{
    public async Task UpAsync()
    {
        var transactionId = Guid.NewGuid().ToString();
        var listFlags = Flag.GetFields();

        foreach (var flag in listFlags)
        {
            var featureFlagEntity = await mongoDbContext.FeatureFlag
                .Where(x => x.Name == flag.Name)
                .Where(x => x.Status == EventStatus.Complete)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            if (featureFlagEntity == null)
            {
                mongoDbContext.FeatureFlag.Add(new FeatureFlagEntity {
                    Name = flag.Name,
                    IsEnabled = flag.Value,
                    Status = EventStatus.Complete,
                    TransactionId = transactionId
                });
            }
        }

        await mongoDbContext.SaveChangesAsync();
    }

    public async Task DownAsync()
    { }
}