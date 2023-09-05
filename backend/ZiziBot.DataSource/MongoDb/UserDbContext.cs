namespace ZiziBot.DataSource.MongoDb;

[Obsolete("Please use MongoDbContextBase directly")]
public class UserDbContext : MongoDbContextBase
{
    public UserDbContext(string connectionStr) : base(connectionStr)
    {
    }
}