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
    public async Task GetSettingPanel(CommandContext context)
    {
        await mediatorService.EnqueueAsync(new PrepareConsoleBotRequest()
        {
            BotToken = context.BotToken,
            Message = context.Message,
            ReplyMessage = true,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("wb")]
    [Command("wd")]
    [Command("wt")]
    public async Task SaveWelcome(CommandContext context)
    {
        await mediatorService.EnqueueAsync(new SaveWelcomeMessageRequest()
        {
            BotToken = context.BotToken,
            Message = context.Message,
            ReplyMessage = true,
            MinimumRole = RoleLevel.ChatAdmin,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("wm")]
    public async Task ShowWelcome(CommandContext context)
    {
        await mediatorService.EnqueueAsync(new ShowWelcomeMessageRequest()
        {
            BotToken = context.BotToken,
            Message = context.Message,
            ReplyMessage = true,
            MinimumRole = RoleLevel.ChatAdmin,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}