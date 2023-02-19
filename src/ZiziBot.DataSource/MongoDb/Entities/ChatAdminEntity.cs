using System.ComponentModel.DataAnnotations.Schema;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("ChatAdmin")]
public class ChatAdminEntity : EntityBase
{
    public long UserId { get; set; }
    public long ChatId { get; set; }
    public ChatMemberStatus Role { get; set; }
}