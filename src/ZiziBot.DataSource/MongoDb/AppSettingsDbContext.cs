using MongoFramework;

namespace ZiziBot.DataSource.MongoDb;

public class AppSettingsDbContext : MongoDbContextBase
{
	public AppSettingsDbContext(string connectionStr) : base(connectionStr) {}

	public MongoDbSet<AppSettings> AppSettings { get; set; }
	public MongoDbSet<BotSettings> BotSettings { get; set; }
	public MongoDbSet<Sudoer> Sudoers { get; set; }
}