using Telegram.Bot.Types;
using ZiziBot.Contracts.Enums;

namespace ZiziBot.Contracts.Dtos;

public class ChatActivityDto
{
    public required long ChatId { get; set; }
    public required long UserId { get; set; }
    public required int MessageId { get; set; }
    public required ChatActivityType ActivityType { get; set; }
    public required Chat Chat { get; set; }
    public required User User { get; set; }
    public EventStatus Status { get; set; }
    public string? TransactionId { get; set; }
}