using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class DebugController : CommandController
{
    private readonly MediatorService _mediatorService;

    public DebugController(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    [Command("about")]
    [Command("start")]
    public async Task GetAbout(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new GetAboutBotRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("id")]
    public async Task GetId(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new GetIdBotRequestModel()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("fid")]
    public async Task GetFileId(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new GetFileIdBotRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("dbg")]
    [Command("yaml")]
    [Command("json")]
    public async Task GetDebug(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new GetDebugBotRequestModel()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTarget = CleanupTarget.FromBot | CleanupTarget.FromSender,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("promote")]
    [Command("demote")]
    public async Task PromoteMember(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new PromoteMemberBotRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
            Promote = data.Message.Text.IsCommand("/promote"),
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [TypedCommand(MessageType.ForumTopicCreated)]
    [TypedCommand(MessageType.ForumTopicEdited)]
    public async Task OnTopicChange(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new ThreadUpdateBotRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}