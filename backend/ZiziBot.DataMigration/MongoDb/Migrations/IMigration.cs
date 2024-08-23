using MongoDB.Driver;

namespace ZiziBot.DataMigration.MongoDb.Migrations;

public interface IMigration
{
    string Id { get; }

    Task UpAsync(IMongoDatabase database);
    Task DownAsync(IMongoDatabase database);
}