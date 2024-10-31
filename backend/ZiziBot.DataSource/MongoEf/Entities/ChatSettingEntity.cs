using MongoDB.EntityFrameworkCore;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("ChatSetting")]
public class ChatSettingEntity : EntityBase
{
    public long ChatId { get; set; }
    public string ChatTitle { get; set; }
    public ChatType ChatType { get; set; }
    public string ChatTypeName { get; set; }

    public bool IsAdmin { get; set; }
    public bool EnableBot { get; set; }
    public long EventLogChatId { get; set; }
    public string? LanguageCode { get; set; }
}