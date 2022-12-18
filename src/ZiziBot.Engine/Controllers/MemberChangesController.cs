using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using MediatR;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Engine.Controllers;

public class MemberChangesController : CommandController
{
    private readonly IMediator _mediator;

    public MemberChangesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [TypedCommand(MessageType.ChatMembersAdded)]
    public void NewChatMembers(MessageData data)
    {
        _mediator.Enqueue(new NewChatMembersRequestModel()
        {
            BotData = data.BotData,
            Message = data.Message
        });
    }
}