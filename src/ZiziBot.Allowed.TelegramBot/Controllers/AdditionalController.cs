using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class AdditionalController : CommandController
{
    private readonly MediatorService _mediatorService;

    public AdditionalController(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    [Command("afk")]
    public async Task Afk(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new SetAfkRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            Reason = data.Params,
            ReplyMessage = true,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

}