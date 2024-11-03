using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.DataSource.MongoEf;

public class MongoEfContext : DbContext
{
    readonly string _connectionString = EnvUtil.GetEnv(Env.MONGODB_CONNECTION_STRING, throwIsMissing: true);

    public DbSet<SudoerEntity> Sudoers { get; set; }
    public DbSet<AppSettingsEntity> AppSettings { get; set; }
    public DbSet<ApiKeyEntity> ApiKey { get; set; }
    public DbSet<BotSettingsEntity> BotSettings { get; set; }
    public DbSet<BotCommandEntity> BotCommand { get; set; }

    public DbSet<FeatureAssignmentEntity> FeatureAssignment { get; set; }
    public DbSet<FeatureFlagEntity> FeatureFlag { get; set; }
    public DbSet<FeatureRolloutCategoryEntity> FeatureRolloutCategory { get; set; }
    public DbSet<FeatureRolloutEntity> FeatureRollout { get; set; }

    public DbSet<ChatRestrictionEntity> ChatRestriction { get; set; }
    public DbSet<ChatAdminEntity> ChatAdmin { get; set; }
    public DbSet<ChatSettingEntity> ChatSetting { get; set; }
    public DbSet<ChatActivityEntity> ChatActivity { get; set; }
    public DbSet<GroupTopicEntity> GroupTopic { get; set; }
    public DbSet<WordFilterEntity> WordFilter { get; set; }
    public DbSet<GlobalBanEntity> GlobalBan { get; set; }
    public DbSet<BotUserEntity> BotUser { get; set; }
    public DbSet<NoteEntity> Note { get; set; }
    public DbSet<WelcomeMessageEntity> WelcomeMessage { get; set; }
    public DbSet<DashboardSessionEntity> DashboardSessions { get; set; }

    public DbSet<AfkEntity> Afk { get; set; }

    public DbSet<WebhookChatEntity> WebhookChat { get; set; }
    public DbSet<WebhookHistoryEntity> WebhookHistory { get; set; }

    public DbSet<RssSettingEntity> RssSetting { get; set; }
    public DbSet<RssHistoryEntity> RssHistory { get; set; }

    public DbSet<ChannelMapEntity> ChannelMap { get; set; }
    public DbSet<ChannelPostEntity> ChannelPost { get; set; }

    public DbSet<MirrorUserEntity> MirrorUser { get; set; }
    public DbSet<MirrorApprovalEntity> MirrorApproval { get; set; }
    public DbSet<MirrorActivityEntity> MirrorActivity { get; set; }

    public DbSet<BangHasan_ShalatCityEntity> BangHasan_ShalatCity { get; set; }

    public DbSet<JadwalSholatOrg_CityEntity> JadwalSholatOrg_City { get; set; }
    public DbSet<JadwalSholatOrg_ScheduleEntity> JadwalSholatOrg_Schedule { get; set; }
    public DbSet<JadwalSholatOrg_ChatCityEntity> JadwalSholatOrg_ChatCity { get; set; }

    public DbSet<PendekinMapEntity> PendekinMap { get; set; }

    public DbSet<TonjooAwbEntity> TonjooAwb { get; set; }
    public DbSet<BinderByteCheckAwbEntity> BinderByteCheckAwb { get; set; }

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

    public async Task<List<TEntity>> GetListAsync<TEntity>() where TEntity : EntityBase
    {
        return await Set<TEntity>().Where(x => x.Status == EventStatus.Complete).ToListAsync();
    }

    public async Task<string> ExportAllAsync<T>() where T : class, new()
    {
        var exportPath = PathConst.MONGODB_BACKUP.EnsureDirectory();
        var data = await Set<T>().ToListAsync();
        var entityName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name!;

        var path = Path.Combine(exportPath, entityName + ".csv").EnsureDirectory();

        data.WriteToCsvFile(path);

        return path;
    }

    void EnsureTimestamp()
    {
        var entries = ChangeTracker.Entries<EntityBase>()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

        foreach (var entityEntry in entries)
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    entityEntry.Entity.CreatedDate = DateTime.UtcNow;
                    entityEntry.Entity.UpdatedDate = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entityEntry.Entity.UpdatedDate = DateTime.UtcNow;
                    break;

                // case EntityState.Deleted:
                //     entityEntry.State = EntityState.Modified; // prevent hard delete
                //     entityEntry.Entity.Status = EventStatus.Deleted; // mark as deleted
                //     entityEntry.Entity.UpdatedDate = DateTime.UtcNow;
                //     break;
            }
        }
    }
}