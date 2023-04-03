using System.ComponentModel.DataAnnotations.Schema;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("BotCommand")]
public class BotCommandEntity : EntityBase
{
    public string Command { get; set; }
    public string Description { get; set; }
    public BotCommandScopeType Scope { get; set; }
}