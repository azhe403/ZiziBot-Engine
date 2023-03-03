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
    public async Task SubmitMirrorPayment(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new VerifyPaymentRequestModel()
        {
            BotToken = data.Options.Token,
            ReplyMessage = true,
            Message = data.Message,
            Payload = data.Params,
            MinimumRole = RoleLevel.Sudo,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}