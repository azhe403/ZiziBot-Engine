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

        var isRoleMeet = request.MinimumRole switch {
            RoleLevel.Sudo => await sudoService.IsSudoAsync(request.UserId),
            RoleLevel.ChatAdmin => await telegramService.CheckAdministration(),
            RoleLevel.ChatCreator => await telegramService.CheckChatCreator(),
            RoleLevel.ChatAdminOrPrivate => await telegramService.CheckChatAdminOrPrivate(),
            RoleLevel.User => true,
            RoleLevel.Guest => true,
            _ => false,
        };

        logger.LogWarning("The minimum role for {Name} for UserId: {UserId} in ChatId: {ChatId} should have role minimum {Role}? {Result}",
            typeof(TRequest).Name, request.UserId, request.ChatId, request.MinimumRole, isRoleMeet);

        if (isRoleMeet)
            return await next();

        return new TResponse();
    }
}