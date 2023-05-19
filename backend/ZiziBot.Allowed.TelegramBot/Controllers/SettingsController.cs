using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class SettingsController : CommandController
{
    private readonly MediatorService _mediatorService;

    public SettingsController(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    [Command("settings")]
    public async Task GetSettingPanel(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new GetSettingPanelBotRequestModel()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyToMessageId = data.Message.MessageId
        });
    }
}