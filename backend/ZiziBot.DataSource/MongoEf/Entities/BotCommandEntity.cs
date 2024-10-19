using MongoDB.EntityFrameworkCore;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("BotCommand")]
public class BotCommandEntity : EntityBase
{
    public string Command { get; set; }
    public string Description { get; set; }
    public BotCommandScopeType Scope { get; set; }
}