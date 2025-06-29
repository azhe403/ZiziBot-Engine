using JetBrains.Annotations;
using MongoDB.Driver;

namespace ZiziBot.Database.MongoDb.Interfaces;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IMigrationBase
{
    Task UpAsync(IMongoDatabase db);
    Task DownAsync(IMongoDatabase db);
}