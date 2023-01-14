using MediatR;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Pipelines;

public class CheckDashboardSessionRequestDto : IRequest<bool>
{
    public long UserId { get; set; }
    public string SessionId { get; set; }
}

public class CheckDashboardSessionRequestHandler : IRequestHandler<CheckDashboardSessionRequestDto, bool>
{
    private readonly ILogger<CheckDashboardSessionRequestHandler> _logger;
    private readonly UserDbContext _userDbContext;

    public CheckDashboardSessionRequestHandler(ILogger<CheckDashboardSessionRequestHandler> logger, UserDbContext userDbContext)
    {
        _logger = logger;
        _userDbContext = userDbContext;
    }

    public async Task<bool> Handle(CheckDashboardSessionRequestDto request, CancellationToken cancellationToken)
    {
        var session = await _userDbContext.DashboardSessions
            .Where(session =>
                session.SessionId == request.SessionId &&
                session.TelegramUserId == request.UserId
            )
            .AnyAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Session {SessionId} for user {UserId} is? {Session}", request.SessionId, request.UserId, session);

        return session;
    }
}