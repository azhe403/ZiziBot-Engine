using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class DebugController : CommandController
{
    private readonly IMediator _mediator;

    public DebugController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Command("id")]
    public async Task GetId(MessageData data)
    {
        await _mediator.EnqueueAsync(
            new GetIdRequestModel()
            {
                Options = data.Options,
                Message = data.Message,
                ReplyToMessageId = data.Message.MessageId,
            }
        );
    }

    [Command("weblogin")]
    public async Task WebLogin(MessageData data)
    {
        await _mediator.EnqueueAsync(
            new CreateWebSessionRequestModel()
            {
                Options = data.Options,
                Message = data.Message,
                ReplyMessage = true,
                CleanupTargets = new[]
                {
                    CleanupTarget.FromBot,
                    CleanupTarget.FromSender
                }
            }
        );
    }
}