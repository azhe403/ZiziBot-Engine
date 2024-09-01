using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class CheckUserRolePipelineBehavior<TRequest, TResponse>(
    ILogger<CheckUserRolePipelineBehavior<TRequest, TResponse>> logger,
    TelegramService telegramService,
    SudoService sudoService
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase, new()
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogDebug("Checking Role {Name} for UserId: {UserId} in ChatId: {ChatId}", typeof(TRequest), request.UserId, request.ChatId);

        telegramService.SetupResponse(request);

        var isRoleMeet = await telegramService.ValidateRole();

        logger.LogWarning("The minimum role for {Name} for UserId: {UserId} in ChatId: {ChatId} should have role minimum {Role}? {Result}",
            typeof(TRequest).Name, request.UserId, request.ChatId, request.MinimumRole, isRoleMeet);

        request.PipelineResult.IsRolePassed = isRoleMeet;

        return await next();
    }
}