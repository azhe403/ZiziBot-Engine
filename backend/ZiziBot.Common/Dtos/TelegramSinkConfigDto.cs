namespace ZiziBot.Common.Dtos;

public class TelegramSinkConfigDto
{
    public string? BotToken { get; init; }
    public long? ChatId { get; init; }
    public long? ThreadId { get; set; }
}