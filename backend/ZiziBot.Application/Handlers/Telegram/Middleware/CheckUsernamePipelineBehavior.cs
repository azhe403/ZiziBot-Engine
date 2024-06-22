using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class CheckUsernamePipelineBehavior<TRequest, TResponse>(
    ILogger<CheckUsernamePipelineBehavior<TRequest, TResponse>> logger,
    TelegramService telegramService
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request.Source != ResponseSource.Bot ||
            request.ChannelPostAny != null)
            return await next();

        telegramService.SetupResponse(request);

        logger.LogDebug("Checking Username for UserId: {UserId} in ChatId: {ChatId}", request.UserId, request.ChatId);

        if (await telegramService.UserHasUsername())
        {
            logger.LogDebug("User passed from checking Username for UserId: {UserId} in ChatId: {ChatId}", request.UserId, request.ChatId);

            return await next();
        }

        var fullName = request.User?.GetFullMention();

        var button = new InlineKeyboardMarkup(new[] {
            new[] {
                InlineKeyboardButton.WithUrl("↗️ Pasang Username", UrlConst.TG_APPLY_USERNAME),
                InlineKeyboardButton.WithUrl("↗️ Apply Username", UrlConst.TG_APPLY_USERNAME),
            }
        });

        await telegramService.SendMessageText($"Hai {fullName}, kamu belum mengatur Username, silakan ikuti petunjuk dibawah ini.", button);
        await telegramService.MuteMemberAsync(request.User.Id, TimeSpan.FromMinutes(1));

        throw new BotMiddlewareException<TRequest>("User Not Passed");
    }
}