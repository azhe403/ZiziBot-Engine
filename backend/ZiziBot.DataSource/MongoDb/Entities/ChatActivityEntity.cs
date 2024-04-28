using Telegram.Bot.Types;

namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("ChatActivity")]
public class ChatActivityEntity : EntityBase
{
    public required long ChatId { get; set; }
    public required ChatActivityType ActivityType { get; set; }
    public required Chat Chat { get; set; }
    public required User User { get; set; }
}