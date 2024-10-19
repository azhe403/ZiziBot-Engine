using JetBrains.Annotations;
using MongoDB.Driver;

namespace ZiziBot.DataMigration.MongoDb.Interfaces;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IMigrationBase
{
    string Id { get; }

    Task UpAsync(IMongoDatabase database);
    Task DownAsync(IMongoDatabase database);
}