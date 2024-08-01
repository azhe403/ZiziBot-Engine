using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class CheckAntispamPipelineBehavior<TRequest, TResponse>(
    ILogger<CheckAntispamPipelineBehavior<TRequest, TResponse>> logger,
    AntiSpamService antiSpamService
)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BotRequestBase
    where TResponse : BotResponseBase, new()
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogDebug("Checking antispam for UserId: {UserId} in ChatId: {ChatId}", request.UserId, request.ChatId);

        var response = new BotMiddlewareResponseBase<AntiSpamDto>();
        var antiSpamDto = await antiSpamService.CheckSpamAsync(request.ChatIdentifier, request.UserId);

        response.CanContinue = !antiSpamDto.IsBanAny;

        if (!antiSpamDto.IsBanAny)
            return await next();

        var htmlMessage = HtmlMessage.Empty
            .User(request.UserId, request.User.GetFullName())
            .Text(" is banned from Global Ban");

        response.Message = htmlMessage.ToString();
        response.Result = antiSpamDto;

        return new TResponse();
    }
}