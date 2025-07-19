using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using ZiziBot.Common.Utils;
using ZiziBot.Database.MongoDb.Interfaces;

namespace ZiziBot.Database.MongoDb;

public class MigrationRunner(IServiceProvider serviceProvider, ILogger<MigrationRunner> logger)
{
    private IMongoCollection<BsonDocument>? _migrationCollection;

    public async Task ApplyMigrationAsync()
    {
        logger.LogInformation("Starting Mongodb Migration");
        using var scope = serviceProvider.CreateScope();
        var serviceScope = scope.ServiceProvider;
        var database = serviceScope.GetRequiredService<IMongoDatabase>();
        _migrationCollection = database.GetCollection<BsonDocument>("__migrations");

        var preMigrations = serviceScope.GetServices<IPreMigration>().ToList<IMigrationBase>();
        logger.LogDebug("Applying Pre-Migration. Total: {Count} Migrations", preMigrations.Count);
        await ApplyMigrationInternal(preMigrations);

        var migrations = serviceScope.GetServices<IMigration>().ToList<IMigrationBase>();
        logger.LogDebug("Applying Migration. Total: {Count} Migrations", migrations.Count);
        await ApplyMigrationInternal(migrations);

        var postMigrations = serviceScope.GetServices<IPostMigration>().ToList<IMigrationBase>();
        logger.LogDebug("Applying Post-Migration. Total: {Count} Migrations", postMigrations.Count);
        await ApplyMigrationInternal(postMigrations);

        database.Client.Dispose();

        logger.LogInformation("Mongodb Migration Completed");
    }

    private async Task ApplyMigrationInternal(List<IMigrationBase> migrations)
    {
        _migrationCollection.EnsureNotNull();

        foreach (var migration in migrations)
        {
            var migrationId = Builders<BsonDocument>.Filter.Eq("Name", migration.GetType().Name);
            var applied = await _migrationCollection.Find(migrationId).FirstOrDefaultAsync();

            if (applied == null || migration is not IMigration)
            {
                logger.LogDebug("Applying Migration {Name}", migration.GetType().Name);

                await migration.UpAsync();

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