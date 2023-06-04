using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class CheckUsernamePipeline : IPipelineBehavior<BotMiddlewareRunnerRequest, BotMiddlewareResponseBase<AntiSpamDto>>
{
    private readonly ILogger<CheckUsernamePipeline> _logger;

    public CheckUsernamePipeline(ILogger<CheckUsernamePipeline> logger)
    {
        _logger = logger;
    }

    public async Task<BotMiddlewareResponseBase<AntiSpamDto>> Handle(
        BotMiddlewareRunnerRequest request,
        RequestHandlerDelegate<BotMiddlewareResponseBase<AntiSpamDto>> next,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Checking Username for UserId: {UserId} in ChatId: {ChatId}", request.UserId, request.ChatId);

        if (request.User?.Username != null)
        {
            _logger.LogDebug("User passed from checking Username for UserId: {UserId} in ChatId: {ChatId}", request.UserId, request.ChatId);

            return await next();
        }

        var fullName = request.User?.GetFullMention();

        return new BotMiddlewareResponseBase<AntiSpamDto>()
        {
            CanContinue = false,
            MuteDuration = TimeSpan.FromMinutes(1),
            Message = $"Hai {fullName}, kamu belum mengatur Username, silakan ikuti petunjuk dibawah ini.",
            ReplyMarkup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithUrl("↗️ Pasang Username", UrlConst.TG_APPLY_USERNAME),
                    InlineKeyboardButton.WithUrl("↗️ Apply Username", UrlConst.TG_APPLY_USERNAME),
                }
            })
        };
    }
}