using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class SudoController(
    MediatorService mediatorService
) : BotCommandController
{
    [Command("addsudo")]
    public async Task AddSudo(CommandData data)
    {
        await mediatorService.EnqueueAsync(new AddSudoBotRequestModel() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            CustomUserId = data.Message.Text.GetCommandParamAt<long>(1),
            DeleteAfter = TimeSpan.FromMinutes(1),
            ReplyToMessageId = data.Message.MessageId,
        });
    }
}