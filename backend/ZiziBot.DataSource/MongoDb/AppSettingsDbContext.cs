namespace ZiziBot.DataSource.MongoDb;

[Obsolete("Please use MongoDbContextBase directly")]
public class AppSettingsDbContext : MongoDbContextBase
{
    public AppSettingsDbContext(string connectionStr) : base(connectionStr) { }
}