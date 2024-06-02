using Telegram.Bot.Types;

namespace ZiziBot.Contracts.Dtos;

public class ChatActivityDto
{
    public required long ChatId { get; set; }
    public required ChatActivityType ActivityType { get; set; }
    public required Chat Chat { get; set; }
    public required User User { get; set; }
    public EventStatus Status { get; set; }
    public required string TransactionId { get; set; }
    public required int MessageId { get; set; }
}