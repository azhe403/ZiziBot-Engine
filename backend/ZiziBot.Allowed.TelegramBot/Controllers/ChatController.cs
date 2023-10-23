using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ChatController : CommandController
{
    private readonly MediatorService _mediatorService;

    public ChatController(MediatorService mediatorServiceService)
    {
        _mediatorService = mediatorServiceService;
    }

    [Command("fch")]
    public async Task AddForwardChannelSource(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new ConnectChannelPostRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
            ChannelId = data.Params.GetCommandParamAt<long>(0),
            MinimumRole = RoleLevel.ChatAdminOrPrivate,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [UpdateCommand(UpdateType.ChatJoinRequest)]
    public async Task JoinRequest(UpdateData data)
    {
        await _mediatorService.EnqueueAsync(new ChatJoinBotRequest()
        {
            BotToken = data.Options.Token,
            Update = data.Update,
            DeleteAfter = TimeSpan.FromMinutes(10),
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot
            }
        });
    }

    [TypedCommand(MessageType.ChatMembersAdded)]
    public async Task NewChatMembers(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new NewChatMembersBotRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            NewUser = data.Message.NewChatMembers!,
            DeleteAfter = TimeSpan.FromMinutes(10),
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot
            }
        });
    }

    [UpdateCommand(UpdateType.ChannelPost)]
    [UpdateCommand(UpdateType.EditedChannelPost)]
    public async Task ChannelPost(UpdateData data)
    {
        await _mediatorService.EnqueueAsync(new ForwardChannelPostRequest()
        {
            BotToken = data.Options.Token,
            Update = data.Update
        });
    }
}