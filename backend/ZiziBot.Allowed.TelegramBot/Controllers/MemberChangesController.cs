using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class MemberChangesController : CommandController
{
    private readonly MediatorService _mediatorService;

    public MemberChangesController(MediatorService mediatorServiceService)
    {
        _mediatorService = mediatorServiceService;
    }

    [TypedCommand(MessageType.ChatMembersAdded)]
    public async Task NewChatMembers(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new NewChatMembersRequestModel()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            NewUser = data.Message.NewChatMembers!,
            DeleteAfter = TimeSpan.FromMinutes(10),
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot
            }
        });
    }
}