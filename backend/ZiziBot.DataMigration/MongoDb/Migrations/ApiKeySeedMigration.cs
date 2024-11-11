using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ZiziBot.Contracts.Enums;
using ZiziBot.DataMigration.MongoDb.Interfaces;
using ZiziBot.DataSource.MongoEf;

namespace ZiziBot.DataMigration.MongoDb.Migrations;

public class ApiKeySeedMigration(ILogger<ApiKeySeedMigration> logger, MongoEfContext mongoDbContext) : IMigration
{
    readonly ILogger<ApiKeySeedMigration> _logger = logger;

    public async Task DownAsync(IMongoDatabase db)
    { }

    public async Task UpAsync(IMongoDatabase db)
    {
        mongoDbContext.ApiKey.Add(new() {
            Name = ApiKeyVendor.Internal,
            Category = ApiKeyCategory.Internal,
            ApiKey = "SOMESERCRETKEY",
            Status = EventStatus.InProgress
        });

        await mongoDbContext.SaveChangesAsync();
    }
}