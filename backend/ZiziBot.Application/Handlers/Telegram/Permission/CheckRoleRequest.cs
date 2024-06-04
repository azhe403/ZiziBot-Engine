using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Permission;

public class CheckRoleHandler<TRequest> : IRequestPreProcessor<TRequest> where TRequest : BotRequestBase
{
    private readonly ILogger<CheckRoleHandler<TRequest>> _logger;
    private readonly TelegramService _telegramService;
    private readonly SudoService _sudoService;

    public CheckRoleHandler(
        ILogger<CheckRoleHandler<TRequest>> logger,
        TelegramService telegramService,
        SudoService sudoService
    )
    {
        _logger = logger;
        _telegramService = telegramService;
        _sudoService = sudoService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Checking Role {Name} for UserId: {UserId} in ChatId: {ChatId}", typeof(TRequest), request.UserId, request.ChatId);

        _telegramService.SetupResponse(request);

        var isRoleMeet = request.MinimumRole switch {
            RoleLevel.Sudo => await _sudoService.IsSudoAsync(request.UserId),
            RoleLevel.ChatAdmin => await _telegramService.CheckAdministration(),
            RoleLevel.ChatCreator => await _telegramService.CheckChatCreator(),
            RoleLevel.ChatAdminOrPrivate => await _telegramService.CheckChatAdminOrPrivate(),
            RoleLevel.User => true,
            RoleLevel.Guest => true,
            _ => false,
        };

        _logger.LogInformation("Role {Name} for UserId: {UserId} in ChatId: {ChatId} should have role minimum {Role} => {Result}",
            typeof(TRequest), request.UserId, request.ChatId, request.MinimumRole, isRoleMeet);

        if (!isRoleMeet)
            throw new MinimumRoleException<TRequest>(request.UserId, request.ChatIdentifier, request.MinimumRole.ToString());
    }
}