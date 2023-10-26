using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using ZiziBot.Application.Handlers.Telegram.Rss;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class RssController : CommandController
{
    private readonly MediatorService _mediatorService;

    public RssController(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    [Command("rss")]
    public async Task AddRss(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new AddRssRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            MinimumRole = RoleLevel.ChatAdminOrPrivate,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}