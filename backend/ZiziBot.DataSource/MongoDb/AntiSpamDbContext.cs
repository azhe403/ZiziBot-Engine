namespace ZiziBot.DataSource.MongoDb;

public class AntiSpamDbContext : MongoDbContextBase
{
    public AntiSpamDbContext(string connectionStr) : base(connectionStr)
    {
    }

    // public MongoDbSet<GlobalBanEntity> GlobalBan { get; set; }
}