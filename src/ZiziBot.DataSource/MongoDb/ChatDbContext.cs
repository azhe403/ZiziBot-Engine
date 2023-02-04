using MongoFramework;

namespace ZiziBot.DataSource.MongoDb;

public class ChatDbContext : MongoDbContextBase
{

    public ChatDbContext(string connectionStr) : base(connectionStr)
    {
    }

    public MongoDbSet<NoteEntity> Note { get; set; }
}