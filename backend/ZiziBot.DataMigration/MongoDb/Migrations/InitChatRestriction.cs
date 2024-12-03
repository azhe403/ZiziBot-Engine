using MongoDB.Driver;
using ZiziBot.Contracts.Enums;
using ZiziBot.DataMigration.MongoDb.Interfaces;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.DataMigration.MongoDb.Migrations;

public class InitChatRestriction(MongoEfContext mongoEfContext) : IMigration
{
    public async Task DownAsync(IMongoDatabase db)
    { }

    public async Task UpAsync(IMongoDatabase db)
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

        await mongoEfContext.SaveChangesAsync();
    }
}