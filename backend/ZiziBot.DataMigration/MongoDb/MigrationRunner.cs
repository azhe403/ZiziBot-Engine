using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using ZiziBot.DataMigration.MongoDb.Migrations;

namespace ZiziBot.DataMigration.MongoDb;

public class MigrationRunner(IServiceProvider serviceProvider, IMongoDatabase database, ILogger<MigrationRunner> logger)
{
    private readonly IMongoCollection<BsonDocument> _migrationCollection = database.GetCollection<BsonDocument>("__migrations");

    public async Task ApplyMigrationAsync()
    {
        var migrations = serviceProvider.GetServices<IMigration>();
        foreach (var migration in migrations)
        {
            var migrationId = Builders<BsonDocument>.Filter.Eq("Name", migration.GetType().Name);
            var applied = await _migrationCollection.Find(migrationId).FirstOrDefaultAsync();

            if (applied == null)
            {
                logger.LogDebug("Applying Migration {Name}", migration.GetType().Name);

                await migration.UpAsync(database);

                var migrationHistory = new BsonDocument() {
                    { "_id", migration.Id },
                    { "Name", migration.GetType().Name },
                    { "AppliedAt", DateTime.UtcNow }
                };

                await _migrationCollection.InsertOneAsync(migrationHistory);
                logger.LogDebug("Migration {Name} applied", migration.GetType().Name);
            }
            else
            {
                logger.LogDebug("Migration {Name} already applied", migration.GetType().Name);
            }
        }
    }

    public async Task RollbackMigrationAsync()
    {
        // TODO. rollback migration
    }
}