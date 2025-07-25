using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ZiziBot.Common.Types;

namespace ZiziBot.Application.Core;

public class BotRequestBase : IRequest<BotResponseBase>
{
    public RoleLevel MinimumRole { get; set; }
    public List<RoleLevel> RolesLevels { get; set; } = new();

    public string BotToken { get; set; }

    public Update? Update { get; set; }

    public ChatJoinRequest? ChatJoinRequest => Update?.ChatJoinRequest;

    public Message? Message { get; set; }

    public Message? ReplyToMessage => Message?.ReplyToMessage?.Type is not (MessageType.ForumTopicCreated or MessageType.ForumTopicEdited) ?
        Message?.ReplyToMessage :
        default;

    public Message? ChannelPost => Update?.ChannelPost;
    public Message? ChannelPostEdited => Update?.EditedChannelPost;
    public Message? ChannelPostAny => ChannelPost ?? ChannelPostEdited;

    public Chat? Chat => Message?.Chat ?? null;

    public ForumTopicCreated? ForumTopicCreated => Message?.ForumTopicCreated;
    public ForumTopicEdited? ForumTopicEdited => Message?.ForumTopicEdited;
    public string? EditedTopicName => ReplyToMessage?.ForumTopicEdited?.Name ?? ForumTopicEdited?.Name;
    public string? CreatedTopicName => ReplyToMessage?.ForumTopicCreated?.Name ?? ForumTopicCreated?.Name;
    public string? TopicName => EditedTopicName ?? CreatedTopicName;

    public CallbackQuery? CallbackQuery { get; set; }

    public InlineQuery? InlineQuery { get; set; }
    public string InlineCommand => InlineQuery?.Query.GetInlineQueryAt<string>(0) ?? string.Empty;
    public string? InlineParam => InlineQuery?.Query.Replace(InlineCommand, "").Trim();

    public DateTime MessageDate => Message?.Date ?? Message?.EditDate ?? DateTime.UtcNow;

    public string? MessageText => Message?.Text;
    public string[]? MessageTexts => Message?.Text?.Split(" ");
    public string[]? RepliedMessageTexts => ReplyToMessage?.Text?.Split(" ");

    public string? Command => MessageTexts?.FirstOrDefault();
    public string Param => MessageTexts?.Skip(1).StrJoin(" ") ?? "";
    public IEnumerable<string>? Params => MessageTexts?.Skip(1);
    public string CallbackQueryId => CallbackQuery?.Id ?? string.Empty;

    public ChatId ChatId => ChatJoinRequest?.Chat.Id ?? Message?.Chat.Id ?? 0;
    public int MessageThreadId => Message?.MessageThreadId ?? 0;
    public long ChatIdentifier => ChatId.Identifier ?? 0;
    public ChatType ChatType => Message?.Chat.Type ?? default;
    public string ChatTitle => Message?.Chat.Title ?? Message?.From?.FirstName ?? Message?.From?.Username ?? Message?.From?.LastName ?? "Unknown";

    public User User => ChatJoinRequest?.From ?? Message?.From ?? CallbackQuery?.From ?? InlineQuery?.From ?? new();
    public User? ReplyToUser => ReplyToMessage?.From;

    public long UserId => User.Id;
    public string FirstName => User.FirstName;
    public string? LastName => User.LastName;
    public string? Username => User.Username;
    public string UserFullName => $"{User.FirstName} {User.LastName}".Trim();
    public string UserLanguageCode => User.LanguageCode ?? "en";

    public string Text { get; set; }

    public DateTime RequestDate => DateTime.UtcNow;

    public int MessageId => Message?.MessageId ?? 0;
    public int ReplyToMessageId { get; set; }

    public bool ReplyMessage { get; set; } = true;
    public bool IsChannel => Update?.ChannelPost != null || Update?.EditedChannelPost != null;
    public bool IsGroup => ChatType is ChatType.Group or ChatType.Supergroup;
    public bool IsPrivateChat => Message?.Chat.Type == ChatType.Private;

    public ExecutionStrategy ExecutionStrategy { get; set; }
    public CleanupTarget CleanupTarget { get; set; }

    public ResponseSource Source { get; set; } = ResponseSource.Bot;

    public BotPipelineResult PipelineResult { get; set; } = new();

    public CleanupTarget[] CleanupTargets { get; set; } = new[] {
        CleanupTarget.None
    };

    public TimeSpan DeleteAfter { get; set; } = TimeSpan.FromMinutes(1);
    public string TransactionId { get; set; } = Guid.NewGuid().ToString();
}