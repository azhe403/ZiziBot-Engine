using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PingController : CommandController
{
    private readonly MediatorService _mediatorService;

    public PingController(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    [Command("ping")]
    [TextCommand("ping")]
    public async Task Ping(MessageData data)
    {
        await _mediatorService.EnqueueAsync(
            new PingRequestModel()
            {
                BotToken = data.Options.Token,
                Message = data.Message,
                DeleteAfter = TimeSpan.FromMinutes(1),
                ReplyMessage = true,
                CleanupTargets = new[]
                {
                    CleanupTarget.FromBot,
                    CleanupTarget.FromSender
                }
            }
        );
    }

    [CallbackQuery(CallbackConst.BOT)]
    public async Task PingCallback(CallbackQueryData data, PingCallbackQueryModel model)
    {
        await _mediatorService.EnqueueAsync(
            new PingCallbackRequestModel()
            {
                BotToken = data.Options.Token,
                CallbackQuery = data.CallbackQuery,
                ExecutionStrategy = ExecutionStrategy.Hangfire
            }
        );
    }

    [DefaultCommand]
    [TextCommand()]
    public async Task Default(MessageData data)
    {
        await _mediatorService.EnqueueAsync(
            new DefaultRequestModel()
            {
                BotToken = data.Options.Token,
                Message = data.Message
            }
        );

        await _mediatorService.EnqueueAsync(
            new FindNoteRequest()
            {
                BotToken = data.Options.Token,
                Message = data.Message,
                ReplyMessage = true,
                CleanupTargets = new[]
                {
                    CleanupTarget.FromBot,
                    CleanupTarget.FromSender
                }
            }
        );
    }
}