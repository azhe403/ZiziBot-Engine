using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PingController(
    MediatorService mediatorService
) : BotCommandController
{
    [Command("ping")]
    [TextCommand("ping")]
    public async Task Ping(CommandData data)
    {
        await mediatorService.EnqueueAsync(new PingBotRequestModel() {
            BotToken = data.BotToken,
            Message = data.Message,
            DeleteAfter = TimeSpan.FromMinutes(1),
            ReplyMessage = true,
            CleanupTargets = [
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            ]
        });
    }

    [Callback(CallbackConst.PING)]
    public async Task PingCallback(CommandData data)
    {
        await mediatorService.EnqueueAsync(new PingCallbackBotRequestModel() {
            BotToken = data.BotToken,
            CallbackQuery = data.CallbackQuery,
            ExecutionStrategy = ExecutionStrategy.Hangfire
        });
    }

    [DefaultCommand]
    public async Task Default(CommandData data)
    {
        await mediatorService.EnqueueAsync(new DefaultBotRequestModel() {
            BotToken = data.BotToken,
            Message = data.Message
        });
    }
}