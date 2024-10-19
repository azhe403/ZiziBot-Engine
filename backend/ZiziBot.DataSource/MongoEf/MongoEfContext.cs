using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.DataSource.MongoEf;

public class MongoEfContext : DbContext
{
    private readonly string _connectionString = EnvUtil.GetEnv(Env.MONGODB_CONNECTION_STRING, throwIsMissing: true);

    public DbSet<SudoerEntity> Sudoers { get; set; }
    public DbSet<AppSettingsEntity> AppSettings { get; set; }
    public DbSet<BotSettingsEntity> BotSettings { get; set; }

    public DbSet<FeatureFlagEntity> FeatureFlag { get; set; }

    public DbSet<ChatRestrictionEntity> ChatRestriction { get; set; }
    public DbSet<ChatAdminEntity> ChatAdmin { get; set; }
    public DbSet<ChatSettingEntity> ChatSetting { get; set; }
    public DbSet<ChatActivityEntity> ChatActivity { get; set; }
    public DbSet<WordFilterEntity> WordFilter { get; set; }
    public DbSet<GlobalBanEntity> GlobalBan { get; set; }
    public DbSet<BotUserEntity> BotUser { get; set; }
    public DbSet<NoteEntity> Note { get; set; }
    public DbSet<DashboardSessionEntity> DashboardSessions { get; set; }

    public DbSet<WebhookChatEntity> WebhookChat { get; set; }
    public DbSet<WebhookHistoryEntity> WebhookHistory { get; set; }

    public DbSet<MirrorActivityEntity> MirrorActivity { get; set; }

    public DbSet<BangHasan_ShalatCityEntity> BangHasan_ShalatCity { get; set; }

    public DbSet<PendekinMapEntity> PendekinMap { get; set; }

    public DbSet<TonjooAwbEntity> TonjooAwb { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conn = MongoUrl.Create(_connectionString);

        optionsBuilder.UseMongoDB(_connectionString, conn.DatabaseName);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        EnsureTimestamp();

        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
    {
        EnsureTimestamp();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void EnsureTimestamp()
    {
        var entries = ChangeTracker.Entries<EntityBase>()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

        foreach (var entityEntry in entries)
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    entityEntry.Entity.CreatedDate = DateTime.UtcNow;
                    entityEntry.Entity.UpdatedDate = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entityEntry.Entity.UpdatedDate = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    entityEntry.State = EntityState.Modified; // prevent hard delete
                    entityEntry.Entity.Status = EventStatus.Deleted; // mark as deleted
                    entityEntry.Entity.UpdatedDate = DateTime.UtcNow;
                    break;
            }
    }
}