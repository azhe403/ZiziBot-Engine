using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class OcrController(
    MediatorService mediatorService
) : BotCommandController
{
    [Command("ocr")]
    public async Task Ocr(CommandData data)
    {
        await mediatorService.EnqueueAsync(new OcrBotRequest() {
                BotToken = data.BotToken,
                Message = data.Message,
                ReplyMessage = true,
                DeleteAfter = TimeSpan.FromHours(1)
            }
        );
    }
}