using Allowed.Telegram.Bot.Models;
using MediatR;
using Telegram.Bot.Types;

namespace ZiziBot.Contracts.MediatR;

public class RequestBase : IRequest<ResponseBase>
{
    public BotData BotData { get; set; }
    public Message Message { get; set; }

    public string Text { get; set; }

    public DateTime RequestTime = DateTime.UtcNow;
    public int ReplyToMessageId { get; set; }
    public bool DirectAction { get; set; }
    public TimeSpan DeleteAfter { get; set; }
}