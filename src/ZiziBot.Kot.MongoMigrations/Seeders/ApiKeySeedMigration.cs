using Kot.MongoDB.Migrations;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ZiziBot.Kot.MongoMigrations.Seeders;

public class ApiKeySeedMigration : MongoMigration
{
    private readonly ILogger<ApiKeySeedMigration> _logger;
    private readonly UserDbContext _userDbContext;
    private static DatabaseVersion DBVersion => new("232.26.3");

    public ApiKeySeedMigration(ILogger<ApiKeySeedMigration> logger, UserDbContext userDbContext) : base(DBVersion)
    {
        _logger = logger;
        _userDbContext = userDbContext;
    }

    public override async Task DownAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
    }

    public override async Task UpAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken)
    {
        _userDbContext.ApiKey.Add(new ApiKeyEntity()
        {
            Name = "SAMPLE_API_KEY",
            Category = "INTERNAL",
            ApiKey = Guid.NewGuid().ToString(),
            Status = (int) EventStatus.InProgress
        });

        await _userDbContext.SaveChangesAsync(cancellationToken);
    }
}