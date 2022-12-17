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
    [AccessLevel(AccessLevel.User)]
    public void Ping(MessageData data)
    {
        _mediator.Enqueue(new PingRequestModel()
            {
                BotData = data.BotData,
                Message = data.Message
            }
        );
    }
}