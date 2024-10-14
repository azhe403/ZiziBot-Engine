using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class BanController(
    MediatorService mediatorService
) : BotCommandController
{
    [Command("gban")]
    public async Task AddGlobalBan(CommandData data)
    {
        await mediatorService.EnqueueAsync(new AddBanBotRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            DeleteAfter = TimeSpan.FromHours(1),
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}