using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class OcrController : CommandController
{
    private readonly IMediator _mediator;

    public OcrController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Command("ocr")]
    public async Task Ocr(MessageData data)
    {
        await _mediator.EnqueueAsync(
            new OcrRequestModel()
            {
                Options = data.Options,
                Message = data.Message,
                ReplyMessage = true,
                DeleteAfter = TimeSpan.FromHours(1)
            }
        );
    }
}