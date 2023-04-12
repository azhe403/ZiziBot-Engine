using MongoFramework;

namespace ZiziBot.DataSource.MongoDb;

public class AppSettingsDbContext : MongoDbContextBase
{
    public AppSettingsDbContext(string connectionStr) : base(connectionStr) { }

    public MongoDbSet<AppSettingsEntity> AppSettings { get; set; }
    public MongoDbSet<BotSettingsEntity> BotSettings { get; set; }
    public MongoDbSet<BotCommandEntity> BotCommand { get; set; }
    public MongoDbSet<SudoerEntity> Sudoers { get; set; }
}