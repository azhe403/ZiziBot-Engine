using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

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

    [Command("id")]
    public async Task GetId(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new GetIdRequestModel()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyToMessageId = data.Message.MessageId,
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
        await _mediatorService.EnqueueAsync(new GetDebugRequestModel()
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

    [Command("console")]
    public async Task OpenConsole(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new CreateWebSessionRequestModel()
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