using Telegram.Bot.Types.Enums;

namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("ChatRestriction")]
public class ChatRestrictionEntity : EntityBase
{
    public long UserId { get; set; }
    public long ChatId { get; set; }
}