using Kot.MongoDB.Migrations;
using MongoDB.Driver;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Kot.MongoMigrations.Seeders;

public class ChatRestrictionSeedMigration(MongoEfContext mongoEfContext) : MongoMigration(DBVersion)
{
    static DatabaseVersion DBVersion => new("238.7.1");

    public override async Task DownAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken)
    { }

    public override async Task UpAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken)
    {
        mongoEfContext.ChatRestriction.AddRange(new List<ChatRestrictionEntity>() {
            new() {
                UserId = 123,
                ChatId = -1001404591750,
                Status = EventStatus.Complete,
                TransactionId = Guid.NewGuid().ToString()
            },
            new() {
                UserId = 123,
                ChatId = -1001835114370,
                Status = EventStatus.Complete,
                TransactionId = Guid.NewGuid().ToString()
            }
        });

        await mongoEfContext.SaveChangesAsync(cancellationToken);
    }
}