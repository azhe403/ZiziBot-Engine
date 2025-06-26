using MongoDB.Driver;
using ZiziBot.Database.MongoDb.Interfaces;

namespace ZiziBot.Database.MongoDb.Migrations;

public class FinishMigration : IPostMigration
{
    public async Task UpAsync(IMongoDatabase db)
    { }

    public async Task DownAsync(IMongoDatabase db)
    { }
}