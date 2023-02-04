using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PingController : CommandController
{
    private readonly IMediator _mediator;

    public PingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Command("ping")]
    public async Task Ping(MessageData data)
    {
        await _mediator.EnqueueAsync(
            new PingRequestModel()
            {
                BotToken = data.Options.Token,
                Message = data.Message,
                DeleteAfter = TimeSpan.FromMinutes(1),
                ReplyToMessageId = data.Message.MessageId,
                CleanupTargets = new[]
                {
                    CleanupTarget.FromBot,
                    CleanupTarget.FromSender
                }
            }
        );
    }

    [DefaultCommand]
    [TextCommand()]
    public async Task Default(MessageData data)
    {
        await _mediator.EnqueueAsync(
            new DefaultRequestModel()
            {
                BotToken = data.Options.Token,
                Message = data.Message
            }
        );
    }
}