namespace ZiziBot.DataSource.MongoDb;

public class GroupDbContext : MongoDbContextBase
{

    public GroupDbContext(string connectionStr) : base(connectionStr)
    {
    }

    // public MongoDbSet<ChatAdminEntity> GroupAdmin { get; set; }
    // public MongoDbSet<GroupTopicEntity> GroupTopic { get; set; }
    // public MongoDbSet<WelcomeMessageEntity> WelcomeMessage { get; set; }
}