namespace ZiziBot.DataSource.MongoDb;

public class AdditionalDbContext : MongoDbContextBase
{
    public AdditionalDbContext(string connectionStr) : base(connectionStr)
    {
    }

    // public MongoDbSet<BinderByteCheckAwbEntity> BinderByteCheckAwb { get; set; }
    // public MongoDbSet<TonjooAwbEntity> TonjooAwb { get; set; }
}