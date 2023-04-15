using MongoFramework;
using MongoFramework.Infrastructure;

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