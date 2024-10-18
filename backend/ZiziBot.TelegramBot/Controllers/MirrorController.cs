using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class MirrorController(
    MediatorService mediatorService
) : BotCommandController
{
    [Command("mp")]
    public async Task SaveMirrorPayment(CommandData data)
    {
        await mediatorService.EnqueueAsync(new SavePaymentBotRequestModel() {
            BotToken = data.BotToken,
            ReplyMessage = true,
            Message = data.Message,
            Payload = data.CommandParam.GetCommandParamAt<string>(0),
            ForUserId = data.CommandParam.GetCommandParamAt<long>(1),
            MinimumRole = RoleLevel.Sudo,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("sp")]
    public async Task SubmitMirrorPayment(CommandData data)
    {
        await mediatorService.EnqueueAsync(new SubmitPaymentBotRequest() {
            BotToken = data.BotToken,
            ReplyMessage = true,
            Message = data.Message,
            Payload = data.CommandParam.GetCommandParamAt<string>(0),
            ForUserId = data.CommandParam.GetCommandParamAt<long>(1),
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("ms")]
    public async Task MirrorSubscription(CommandData data)
    {
        await mediatorService.EnqueueAsync(new GetMirrorSubscriptionBotRequest() {
            BotToken = data.BotToken,
            ReplyMessage = true,
            Message = data.Message,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}