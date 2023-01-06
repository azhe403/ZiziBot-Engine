using MongoFramework;

namespace ZiziBot.DataSource.MongoDb;

public class UserDbContext:MongoDbContextBase
{
    public UserDbContext(string connectionStr) : base(connectionStr)
    {
    }
    
    public MongoDbSet<DashboardSession> DashboardSessions { get; set; }
}