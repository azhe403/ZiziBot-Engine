using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class CheckUsernamePipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
    private readonly ILogger<CheckUsernamePipelineBehavior<TRequest, TResponse>> _logger;
    private readonly TelegramService _telegramService;

    public CheckUsernamePipelineBehavior(ILogger<CheckUsernamePipelineBehavior<TRequest, TResponse>> logger, TelegramService telegramService)
    {
        _logger = logger;
        _telegramService = telegramService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request.Source != ResponseSource.Bot ||
            request.ChannelPostAny != null)
            return await next();

        _logger.LogDebug("Checking Username for UserId: {UserId} in ChatId: {ChatId}", request.UserId, request.ChatId);

        if (request.User?.Username != null)
        {
            _logger.LogDebug("User passed from checking Username for UserId: {UserId} in ChatId: {ChatId}", request.UserId, request.ChatId);

            return await next();
        }

        _telegramService.SetupResponse(request);

        var fullName = request.User?.GetFullMention();

        var button = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("↗️ Pasang Username", UrlConst.TG_APPLY_USERNAME),
                InlineKeyboardButton.WithUrl("↗️ Apply Username", UrlConst.TG_APPLY_USERNAME),
            }
        });

        await _telegramService.SendMessageText($"Hai {fullName}, kamu belum mengatur Username, silakan ikuti petunjuk dibawah ini.", button);
        await _telegramService.MuteMemberAsync(request.User.Id, TimeSpan.FromMinutes(1));

        throw new BotMiddlewareException<TRequest>("User Not Passed");
    }
}