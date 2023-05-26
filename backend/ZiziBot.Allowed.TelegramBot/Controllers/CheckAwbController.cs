using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Enums;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class CheckAwbController : CommandController
{
    private readonly MediatorService _mediatorService;

    public CheckAwbController(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    [TextCommand("jne", Type = ComparisonTypes.Parameterized)]
    public async Task CheckJne(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new CheckResiRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
        });
    }

    [TextCommand("sicepat", Type = ComparisonTypes.Parameterized)]
    public async Task CheckSiCepat(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new CheckResiRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
        });
    }
}