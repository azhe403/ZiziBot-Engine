using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class SettingsController(
    MediatorService mediatorService
) : BotCommandController
{
    [Command("console")]
    [Command("settings")]
    public async Task GetSettingPanel(CommandData data)
    {
        await mediatorService.EnqueueAsync(new PrepareConsoleBotRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("wb")]
    [Command("wd")]
    [Command("wt")]
    public async Task SaveWelcome(CommandData data)
    {
        await mediatorService.EnqueueAsync(new SaveWelcomeMessageRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            MinimumRole = RoleLevel.ChatAdmin,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("wm")]
    public async Task ShowWelcome(CommandData data)
    {
        await mediatorService.EnqueueAsync(new ShowWelcomeMessageRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            MinimumRole = RoleLevel.ChatAdmin,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}