using Telegram.Bot.Types;

namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("ChatActivity")]
public class ChatActivityEntity : EntityBase
{
    public int MessageId { get; set; }
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public ChatActivityType ActivityType { get; set; }
    public required string ActivityTypeName { get; set; }
    public required Chat Chat { get; set; }
    public required User User { get; set; }
}