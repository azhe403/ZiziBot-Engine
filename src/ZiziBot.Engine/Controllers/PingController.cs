using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using JetBrains.Annotations;
using MediatR;

namespace ZiziBot.Engine.Controllers;

[UsedImplicitly]
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
        await _mediator.EnqueueAsync(new PingRequestModel()
            {
                BotData = data.BotData,
                Message = data.Message,
                Mediator = _mediator,
                DeleteAfter = TimeSpan.FromMinutes(1),
                ReplyToMessageId = data.Message.MessageId,
                DirectAction = true
            }
        );
    }
}