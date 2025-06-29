using MongoDB.Driver;
using ZiziBot.Database.MongoDb.Entities;
using ZiziBot.Database.MongoDb.Interfaces;

namespace ZiziBot.Database.MongoDb.Migrations;

public class InitChatRestriction(MongoDbContext mongoDbContext) : IMigration
{
    public async Task DownAsync(IMongoDatabase db)
    { }

    public async Task UpAsync(IMongoDatabase db)
    {
        mongoDbContext.ChatRestriction.AddRange(new List<ChatRestrictionEntity>() {
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

        await mongoDbContext.SaveChangesAsync();
    }
}