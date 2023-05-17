using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class MirrorController : CommandController
{
    private readonly MediatorService _mediatorService;

    public MirrorController(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    [Command("mp")]
    public async Task SaveMirrorPayment(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new SavePaymentBotRequestModel()
        {
            BotToken = data.Options.Token,
            ReplyMessage = true,
            Message = data.Message,
            Payload = data.Params.GetCommandParamAt<string>(0),
            ForUserId = data.Params.GetCommandParamAt<long>(1),
            MinimumRole = RoleLevel.Sudo,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("sp")]
    public async Task SubmitMirrorPayment(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new SubmitPaymentBotRequestModel()
        {
            BotToken = data.Options.Token,
            ReplyMessage = true,
            Message = data.Message,
            Payload = data.Params.GetCommandParamAt<string>(0),
            ForUserId = data.Params.GetCommandParamAt<long>(1),
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("ms")]
    public async Task MirrorSubscription(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new GetMirrorSubscriptionBotRequest()
        {
            BotToken = data.Options.Token,
            ReplyMessage = true,
            Message = data.Message,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}