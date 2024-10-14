using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class AdditionalController(
    MediatorService mediatorService
) : BotCommandController
{
    [Command("afk")]
    public async Task Afk(CommandData data)
    {
        await mediatorService.EnqueueAsync(new SetAfkBotRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            Reason = data.CommandParam,
            ReplyMessage = true,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("webhook")]
    [Command("wh")]
    public async Task CreateWebhook(CommandData data)
    {
        await mediatorService.EnqueueAsync(new CreateWebhookBotRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            MinimumRole = RoleLevel.ChatAdminOrPrivate,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    // [TextCommand("anteraja", Type = ComparisonTypes.Parameterized)]
    // [TextCommand("jne", Type = ComparisonTypes.Parameterized)]
    // [TextCommand("jnt", Type = ComparisonTypes.Parameterized)]
    // [TextCommand("lion", Type = ComparisonTypes.Parameterized)]
    // [TextCommand("ncs", Type = ComparisonTypes.Parameterized)]
    // [TextCommand("tiki", Type = ComparisonTypes.Parameterized)]
    // [TextCommand("trawl", Type = ComparisonTypes.Parameterized)]
    // [TextCommand("trawlbens", Type = ComparisonTypes.Parameterized)]
    // [TextCommand("sicepat", Type = ComparisonTypes.Parameterized)]
    // [TextCommand("wahana", Type = ComparisonTypes.Parameterized)]
    public async Task CheckAwb(CommandData data)
    {
        await mediatorService.EnqueueAsync(new CheckAwbRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
        });
    }
}