using Telegram.Bot.Types.Enums;

namespace ZiziBot.Contracts.Dtos.Entity;

public class ChatSettingDto
{
    public long ChatId { get; set; }
    public string ChatTitle { get; set; }
    public ChatType ChatType { get; set; }
}