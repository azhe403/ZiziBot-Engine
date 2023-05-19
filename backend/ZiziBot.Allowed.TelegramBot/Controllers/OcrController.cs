using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class OcrController : CommandController
{
    private readonly MediatorService _mediatorService;

    public OcrController(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    [Command("ocr")]
    public async Task Ocr(MessageData data)
    {
        await _mediatorService.EnqueueAsync(
            new OcrBotRequestModel()
            {
                BotToken = data.Options.Token,
                Message = data.Message,
                ReplyMessage = true,
                DeleteAfter = TimeSpan.FromHours(1)
            }
        );
    }
}