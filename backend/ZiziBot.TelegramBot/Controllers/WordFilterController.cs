using ZiziBot.Application.Handlers.Telegram.WordFilter;
using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class WordFilterController(
    MediatorService mediatorService
) : BotCommandController
{
    [Command("awf")]
    public async Task AddBadWordCommand(CommandData data)
    {
        await mediatorService.EnqueueAsync(new AddWordFilterRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            MinimumRole = RoleLevel.Sudo,
            Word = data.Message.Text.GetCommandParamAt<string>(1),
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("dwf")]
    public async Task DisableBadWordCommand(CommandData data)
    {
        await mediatorService.EnqueueAsync(new DisableWordFilterRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            MinimumRole = RoleLevel.Sudo,
            Word = data.Message.Text.GetCommandParamAt<string>(1),
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}