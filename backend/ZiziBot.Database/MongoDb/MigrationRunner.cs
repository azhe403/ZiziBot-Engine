using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using ZiziBot.Database.MongoDb.Interfaces;

namespace ZiziBot.Database.MongoDb;

public class MigrationRunner(IServiceProvider serviceProvider, IMongoDatabase database, ILogger<MigrationRunner> logger)
{
    private readonly IMongoCollection<BsonDocument> _migrationCollection = database.GetCollection<BsonDocument>("__migrations");

    public async Task ApplyMigrationAsync()
    {
        IEnumerable<IMigrationBase> preMigrations = serviceProvider.GetServices<IPreMigration>().ToList();
        logger.LogDebug("Applying Pre-Migration. Total: {Count} Migrations", preMigrations.Count());
        await ApplyMigrationInternal(preMigrations);

        var migrations = serviceProvider.GetServices<IMigration>().ToList();
        logger.LogDebug("Applying Migration. Total: {Count} Migrations", migrations.Count());
        await ApplyMigrationInternal(migrations);

        IEnumerable<IMigrationBase> postMigrations = serviceProvider.GetServices<IPostMigration>().ToList();
        logger.LogDebug("Applying Post-Migration. Total: {Count} Migrations", postMigrations.Count());
        await ApplyMigrationInternal(postMigrations);
    }

    private async Task ApplyMigrationInternal(IEnumerable<IMigrationBase> migrations)
    {
        foreach (var migration in migrations)
        {
            var migrationId = Builders<BsonDocument>.Filter.Eq("Name", migration.GetType().Name);
            var applied = await _migrationCollection.Find(migrationId).FirstOrDefaultAsync();

            if (applied == null || migration is not IMigration)
            {
                logger.LogDebug("Applying Migration {Name}", migration.GetType().Name);

                await migration.UpAsync(database);

                var migrationHistory = new BsonDocument() {
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