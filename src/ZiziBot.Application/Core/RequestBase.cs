using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Core;

public class RequestBase : IRequest<ResponseBase>
{
    public string BotToken { get; set; }
    public IMediator Mediator { get; set; }

    public Message? Message { get; set; }
    public Message? ReplyToMessage => Message?.ReplyToMessage;

    public string[]? MessageTexts => Message?.Text?.Split(" ");
    public string[]? RepliedMessageTexts => ReplyToMessage?.Text?.Split(" ");

    public ChatId ChatId => Message?.Chat.Id ?? default;
    public long ChatIdentifier => ChatId.Identifier ?? default;
    public ChatType ChatType => Message?.Chat.Type ?? default;
    public string ChatTitle => Message?.Chat.Title ?? Message?.From?.FirstName ?? Message?.From?.Username ?? Message?.From?.LastName ?? "Unknown";

    public long UserId => Message?.From?.Id ?? 0;
    public string UserFullName => $"{Message?.From?.FirstName} {Message?.From?.LastName}".Trim();

    public string Text { get; set; }

    public DateTime RequestDate => DateTime.UtcNow;

    public int MessageId => Message?.MessageId ?? default;
    public int ReplyToMessageId { get; set; }

    public bool ReplyMessage { get; set; }

    public ExecutionStrategy ExecutionStrategy { get; set; }

    public CleanupTarget[] CleanupTargets { get; set; } = new[]
    {
        CleanupTarget.Nothing
    };

    public TimeSpan DeleteAfter { get; set; } = TimeSpan.FromMinutes(1);

    public Message SentMessage { get; set; }
}