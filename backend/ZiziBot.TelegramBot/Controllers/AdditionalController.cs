using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;
using ZiziBot.TelegramBot.Framework.Models.Enums;

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

    [TextCommand("anteraja", ComparisonType.CommandLike)]
    [TextCommand("jne", ComparisonType.CommandLike)]
    [TextCommand("jnt", ComparisonType.CommandLike)]
    [TextCommand("lion", ComparisonType.CommandLike)]
    [TextCommand("ncs", ComparisonType.CommandLike)]
    [TextCommand("tiki", ComparisonType.CommandLike)]
    [TextCommand("trawl", ComparisonType.CommandLike)]
    [TextCommand("trawlbens", ComparisonType.CommandLike)]
    [TextCommand("sicepat", ComparisonType.CommandLike)]
    [TextCommand("wahana", ComparisonType.CommandLike)]
    public async Task CheckAwb(CommandData data)
    {
        await mediatorService.EnqueueAsync(new CheckAwbRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
        });
    }
}