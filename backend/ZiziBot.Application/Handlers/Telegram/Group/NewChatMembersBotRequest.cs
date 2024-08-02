using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using Telegram.Bot.Types;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class NewChatMembersBotRequest : BotRequestBase
{
    public required User[] NewUser { get; set; }
}

[UsedImplicitly]
public class NewChatMembersHandler : IRequestHandler<NewChatMembersBotRequest, BotResponseBase>
{
    private readonly ILogger<NewChatMembersHandler> _logger;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly TelegramService _telegramService;
    private readonly ChatSettingRepository _chatSettingRepository;

    public NewChatMembersHandler(
        ILogger<NewChatMembersHandler> logger,
        MongoDbContextBase mongoDbContext,
        TelegramService telegramService,
        ChatSettingRepository chatSettingRepository
    )
    {
        _logger = logger;
        _mongoDbContext = mongoDbContext;
        _telegramService = telegramService;
        _chatSettingRepository = chatSettingRepository;
    }

    public async Task<BotResponseBase> Handle(NewChatMembersBotRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);
        _logger.LogInformation("New Chat Members. ChatId: {ChatId}", request.ChatId);

        var create = await _chatSettingRepository.MeasureActivity(new ChatActivityDto {
            ChatId = request.ChatIdentifier,
            UserId = request.UserId,
            ActivityType = ChatActivityType.NewChatMember,
            Chat = request.Chat,
            User = request.User,
            Status = EventStatus.Complete,
            TransactionId = request.TransactionId,
            MessageId = request.MessageId
        });

        if (create)
        {
            var message = HtmlMessage.Empty;
            message.Bold("Anti-RAID mode.").Br()
                .Text("Terdeteksi banyak anggota baru masuk ke grub dalam beberapa waktu terakhir. " +
                      "Untuk alasan keamanan, anggota baru yang masuk dalam {COOLDOWN_TIME} akan disenyapkan.").Br();

            await _telegramService.SendMessageAsync(message.ToString());
            await _telegramService.MuteMemberAsync(request.UserId, ValueConst.NEW_MEMBER_RAID_MODE_MUTE_DURATION);

            return _telegramService.Complete();
        }

        var chatTitle = request.ChatTitle;
        var newMemberCount = request.NewUser.Length;
        var allNewMember = request.NewUser.Select(user => user.GetFullMention())
            .Aggregate((s, next) => s + ", " + next);
        var greet = TimeUtil.GetTimeGreet();
        var memberCount = await _telegramService.GetMemberCount();

        var messageTemplate = "Hai {AllNewMember}\n" +
                              "Selamat datang di Kontrakan {ChatTitle}";

        var welcomeMessage = await _mongoDbContext.WelcomeMessage.AsNoTracking()
            .Where(x => x.ChatId == request.ChatIdentifier)
            .Where(x => x.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken);

        if (welcomeMessage != null)
        {
            messageTemplate = welcomeMessage.Text;
        }

        var messageText = messageTemplate.ResolveVariable(new List<(string placeholder, string value)>() {
            ("AllNewMember", allNewMember),
            // ("AllNoUsername", allNoUsername),
            // ("AllNewBot", allNewBot),
            ("ChatTitle", chatTitle),
            ("Greet", greet),
            ("NewMemberCount", newMemberCount.ToString()),
            ("MemberCount", memberCount.ToString())
        });

        return await _telegramService.SendMessageAsync(
            text: messageText,
            replyMarkup: welcomeMessage?.RawButton.ToButtonMarkup(),
            fileId: welcomeMessage?.Media,
            mediaType: (CommonMediaType)welcomeMessage?.DataType
        );
    }
}