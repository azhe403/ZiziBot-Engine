namespace ZiziBot.DataSource.MongoDb;

[Obsolete("Please use MongoDbContextBase directly")]
public class AdditionalDbContext : MongoDbContextBase
{
    public AdditionalDbContext(string connectionStr) : base(connectionStr)
    {
    }
}