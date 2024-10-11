using ZiziBot.Application.Handlers.Telegram.Rss;
using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class RssController(
    MediatorService mediatorService
) : BotCommandController
{
    [Command("rss")]
    public async Task AddRss(CommandData data)
    {
        await mediatorService.EnqueueAsync(new AddRssRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            MinimumRole = RoleLevel.ChatAdminOrPrivate,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}