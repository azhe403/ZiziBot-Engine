using MongoFramework;

namespace ZiziBot.DataSource.MongoDb;

public class AppSettingsDbContext : MongoDbContext
{
    public AppSettingsDbContext(IMongoDbConnection connection) : base(connection)
    {
    }

    public MongoDbSet<AppSettings> AppSettings { get; set; }
    public MongoDbSet<BotSettings> BotSettings { get; set; }
}