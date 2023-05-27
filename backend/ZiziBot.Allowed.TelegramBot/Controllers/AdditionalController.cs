using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Enums;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class AdditionalController : CommandController
{
    private readonly MediatorService _mediatorService;

    public AdditionalController(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    [Command("afk")]
    public async Task Afk(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new SetAfkBotRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            Reason = data.Params,
            ReplyMessage = true,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("webhook")]
    public async Task CreateWebhook(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new CreateWebhookBotRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
            MinimumRole = RoleLevel.ChatAdminOrPrivate,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [TextCommand("anteraja", Type = ComparisonTypes.Parameterized)]
    [TextCommand("jne", Type = ComparisonTypes.Parameterized)]
    [TextCommand("jnt", Type = ComparisonTypes.Parameterized)]
    [TextCommand("lion", Type = ComparisonTypes.Parameterized)]
    [TextCommand("ncs", Type = ComparisonTypes.Parameterized)]
    [TextCommand("tiki", Type = ComparisonTypes.Parameterized)]
    [TextCommand("trawl", Type = ComparisonTypes.Parameterized)]
    [TextCommand("trawlbens", Type = ComparisonTypes.Parameterized)]
    [TextCommand("sicepat", Type = ComparisonTypes.Parameterized)]
    [TextCommand("wahana", Type = ComparisonTypes.Parameterized)]
    public async Task CheckAwb(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new CheckAwbRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
        });
    }
}