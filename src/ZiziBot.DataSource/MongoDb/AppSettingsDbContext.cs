using MongoFramework;
using MongoFramework.Infrastructure;

namespace ZiziBot.DataSource.MongoDb;

public class AppSettingsDbContext : MongoDbContext
{
    public AppSettingsDbContext(string connectionStr) : base(MongoDbConnection.FromConnectionString(connectionStr))
    {
    }

    public MongoDbSet<AppSettings> AppSettings { get; set; }
    public MongoDbSet<BotSettings> BotSettings { get; set; }
    public MongoDbSet<Sudoer> Sudoers { get; set; }

    public override Task SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is EntityBase &&
                e.State is EntityEntryState.Added or EntityEntryState.Updated
            );

        foreach (var entityEntry in entries)
        {
            ((EntityBase)entityEntry.Entity).UpdateTime = DateTime.Now;

            if (entityEntry.State == EntityEntryState.Added)
            {
                ((EntityBase)entityEntry.Entity).CreatedDate = DateTime.Now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}