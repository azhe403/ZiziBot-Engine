using System.Reflection;
using MongoFramework;
using MongoFramework.Infrastructure;
using MongoFramework.Linq;

namespace ZiziBot.DataSource.MongoDb;

public class MongoDbContextBase : MongoDbContext
{
    public MongoDbContextBase(string connectionStr) : base(MongoDbConnection.FromConnectionString(connectionStr))
    {
    }

    public override void SaveChanges()
    {
        EnsureTimestamp();
        base.SaveChanges();
    }

    public override Task SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        EnsureTimestamp();
        return base.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> ExportAllAsync<T>() where T : class, new()
    {
        var data = await Query<T>().ToListAsync();
        var entityName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name!;

        var path = Path.Combine(PathConst.MONGODB_BACKUP, entityName + ".csv").EnsureDirectory();

        data.WriteToCsvFile(path);

        return path;
    }

    private void EnsureTimestamp()
    {
        ChangeTracker.DetectChanges();

        var entries = ChangeTracker
            .Entries()
            .Where(entityEntry =>
                entityEntry.Entity is EntityBase &&
                entityEntry.State is EntityEntryState.Added or EntityEntryState.Updated or EntityEntryState.Deleted
            );

        foreach (var entityEntry in entries)
        {
            ((EntityBase)entityEntry.Entity).UpdatedDate = DateTime.UtcNow;

            switch (entityEntry.State)
            {
                case EntityEntryState.Added:
                    ((EntityBase)entityEntry.Entity).CreatedDate = DateTime.UtcNow;
                    break;
                case EntityEntryState.Deleted:
                    ((EntityBase)entityEntry.Entity).Status = (int)EventStatus.Deleted;
                    break;
            }

        }
    }
}