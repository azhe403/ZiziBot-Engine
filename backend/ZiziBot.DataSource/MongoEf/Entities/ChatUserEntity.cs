using MongoDB.EntityFrameworkCore;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("ChatUser")]
public class ChatUserEntity : EntityBase
{
    public long ChatId { get; set; }
    public ChatType ChatType { get; set; }
    public int MemberCount { get; set; }
    public string? ChatUsername { get; set; }
    public string? ChatTitle { get; set; }
    public bool IsBotAdmin { get; set; }
}