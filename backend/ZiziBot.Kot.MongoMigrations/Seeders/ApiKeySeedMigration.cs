using Kot.MongoDB.Migrations;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ZiziBot.Kot.MongoMigrations.Seeders;

public class ApiKeySeedMigration : MongoMigration
{
    private readonly ILogger<ApiKeySeedMigration> _logger;
    private readonly MongoDbContextBase _mongoDbContext;
    private static DatabaseVersion DBVersion => new("232.26.3");

    public ApiKeySeedMigration(ILogger<ApiKeySeedMigration> logger, MongoDbContextBase mongoDbContext) : base(DBVersion)
    {
        _logger = logger;
        _mongoDbContext = mongoDbContext;
    }

    public override async Task DownAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
    }

    public override async Task UpAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken)
    {
        _mongoDbContext.ApiKey.Add(new ApiKeyEntity()
        {
            Name = "SAMPLE_API_KEY",
            Category = "INTERNAL",
            ApiKey = Guid.NewGuid().ToString(),
            Status = (int)EventStatus.InProgress
        });

        await _mongoDbContext.SaveChangesAsync(cancellationToken);
    }
}