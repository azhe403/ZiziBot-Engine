using MongoFramework;

namespace ZiziBot.DataSource.MongoDb;

public class UserDbContext : MongoDbContextBase
{
    public UserDbContext(string connectionStr) : base(connectionStr)
    {
    }

    public MongoDbSet<DashboardSessionEntity> DashboardSessions { get; set; }
    public MongoDbSet<ApiKeyEntity> ApiKey { get; set; }
    public MongoDbSet<BotUserEntity> BotUser { get; set; }
}