using MongoFramework;
using MongoFramework.Infrastructure;

namespace ZiziBot.DataSource.MongoDb;

public class MongoDbContextBase : MongoDbContext
{
    public MongoDbContextBase(string connectionStr) : base(MongoDbConnection.FromConnectionString(connectionStr))
    {
    }

    public override Task SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var entries = ChangeTracker
            .Entries()
            .Where(
                e =>
                    e.Entity is EntityBase &&
                    e.State is EntityEntryState.Added or EntityEntryState.Updated
            );

        foreach (var entityEntry in entries)
        {
            ((EntityBase)entityEntry.Entity).UpdatedDate = DateTime.Now;

            if (entityEntry.State == EntityEntryState.Added)
            {
                ((EntityBase)entityEntry.Entity).CreatedDate = DateTime.Now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}