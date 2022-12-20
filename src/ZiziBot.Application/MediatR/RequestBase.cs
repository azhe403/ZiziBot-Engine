using Allowed.Telegram.Bot.Models;
using MediatR;
using Telegram.Bot.Types;

namespace ZiziBot.Application.MediatR;

public class RequestBase : IRequest<ResponseBase>
{
    public IMediator Mediator { get; set; }
    public BotData BotData { get; set; }
    public Message Message { get; set; }

    public ChatId ChatId => Message.Chat.Id;
    public string ChatTTitle => Message.Chat.Title ?? Message.From?.FirstName ?? Message.From?.Username ?? Message.From?.LastName ?? "Unknown";

    public long UserId => Message.From?.Id ?? 0;
    public string UserFullName => $"{Message.From?.FirstName} {Message.From?.LastName}".Trim();


    public string Text { get; set; }

    public DateTime RequestTime = DateTime.UtcNow;
    public int ReplyToMessageId { get; set; }

    /// <summary>
    /// If true, the pipelines will be executed in instant-mode, otherwise execution will be enqueued to Hangfire.
    /// </summary>
    public bool DirectAction { get; set; }
    public TimeSpan DeleteAfter { get; set; }
}