using Kot.MongoDB.Migrations;
using MongoDB.Driver;

namespace ZiziBot.Kot.MongoMigrations.Seeders;

public class ChatRestrictionSeedMigration : MongoMigration
{
    private readonly MongoDbContextBase _mongoDbContext;
    private static DatabaseVersion DBVersion => new("238.7.1");

    public ChatRestrictionSeedMigration(MongoDbContextBase mongoDbContext) : base(DBVersion)
    {
        _mongoDbContext = mongoDbContext;
    }

    public override async Task DownAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken)
    {
    }

    public override async Task UpAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken)
    {
        _mongoDbContext.ChatRestriction.AddRange(new List<ChatRestrictionEntity>()
        {
            new()
            {
                UserId = 123,
                ChatId = -1001404591750,
                Status = (int)EventStatus.Complete,
                TransactionId = Guid.NewGuid().ToString()
            },
            new()
            {
                UserId = 123,
                ChatId = -1001835114370,
                Status = (int)EventStatus.Complete,
                TransactionId = Guid.NewGuid().ToString()
            }
        });

        await _mongoDbContext.SaveChangesAsync(cancellationToken);
    }
}