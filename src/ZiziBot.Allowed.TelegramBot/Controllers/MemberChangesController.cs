using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class MemberChangesController : CommandController
{
    private readonly IMediator _mediator;

    public MemberChangesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [TypedCommand(MessageType.ChatMembersAdded)]
    public async Task NewChatMembers(MessageData data)
    {
        await _mediator.EnqueueAsync(new NewChatMembersRequestModel()
        {
            Options = data.Options,
            Message = data.Message,
            DeleteAfter = TimeSpan.FromMinutes(1)
        });
    }
}