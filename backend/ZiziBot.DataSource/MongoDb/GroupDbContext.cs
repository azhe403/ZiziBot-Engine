namespace ZiziBot.DataSource.MongoDb;

[Obsolete("Please use MongoDbContextBase directly")]
public class GroupDbContext : MongoDbContextBase
{

    public GroupDbContext(string connectionStr) : base(connectionStr)
    {
    }
}