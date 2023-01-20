using Allowed.Telegram.Bot.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Core;

public class RequestBase : IRequest<ResponseBase>
{
    public IMediator Mediator { get; set; }
    public SimpleTelegramBotClientOptions Options { get; set; }
    public Message Message { get; set; }

    public ChatId ChatId => Message.Chat.Id;
    public ChatType ChatType => Message.Chat.Type;
    public string ChatTitle => Message.Chat.Title ?? Message.From?.FirstName ?? Message.From?.Username ?? Message.From?.LastName ?? "Unknown";

    public long UserId => Message.From?.Id ?? 0;
    public string UserFullName => $"{Message.From?.FirstName} {Message.From?.LastName}".Trim();

    public string Text { get; set; }

    public DateTime RequestTime => DateTime.UtcNow;
    public int ReplyToMessageId { get; set; }

    public bool DirectAction { get; set; }

    public ExecutionStrategy ExecutionStrategy { get; set; }

    public CleanupStrategy CleanupStrategy { get; set; }
    public TimeSpan DeleteAfter { get; set; } = TimeSpan.FromMinutes(1);

    public Message SentMessage { get; set; }
}