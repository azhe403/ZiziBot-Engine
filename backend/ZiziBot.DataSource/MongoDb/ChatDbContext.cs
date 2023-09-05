namespace ZiziBot.DataSource.MongoDb;

[Obsolete("Please use MongoDbContextBase directly")]
public class ChatDbContext : MongoDbContextBase
{
    public ChatDbContext(string connectionStr) : base(connectionStr)
    {
    }
}