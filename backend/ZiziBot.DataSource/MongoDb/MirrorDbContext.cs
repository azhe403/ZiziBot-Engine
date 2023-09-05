namespace ZiziBot.DataSource.MongoDb;

[Obsolete("Please use MongoDbContextBase directly")]
public class MirrorDbContext : MongoDbContextBase
{
    public MirrorDbContext(string connectionStr) : base(connectionStr)
    {
    }
}