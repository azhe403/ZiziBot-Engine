namespace ZiziBot.DataSource.MongoDb;

public class MirrorDbContext : MongoDbContextBase
{
    public MirrorDbContext(string connectionStr) : base(connectionStr)
    {
    }

    // public MongoDbSet<MirrorUserEntity> MirrorUsers { get; set; }
    // public MongoDbSet<MirrorApprovalEntity> MirrorApproval { get; set; }
}