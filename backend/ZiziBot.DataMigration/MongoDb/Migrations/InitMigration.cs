using MongoDB.Driver;

namespace ZiziBot.DataMigration.MongoDb.Migrations;

public class InitMigration : IMigration
{
    public string Id => nameof(InitMigration);

    public async Task UpAsync(IMongoDatabase database)
    {
    }

    public async Task DownAsync(IMongoDatabase database)
    {
    }
}