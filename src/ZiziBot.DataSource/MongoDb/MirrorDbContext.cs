using MongoFramework;

namespace ZiziBot.DataSource.MongoDb
{
    public class MirrorDbContext : MongoDbContextBase
    {
        public MirrorDbContext(string connectionStr) : base(connectionStr)
        {
        }

        public MongoDbSet<MirrorUser> MirrorUsers { get; set; }
    }
}