using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class CheckDashboardSessionRequestDto : IRequest<CheckDashboardSessionResponseDto>
{
    public long UserId { get; set; }
    public string SessionId { get; set; }
}

public class CheckDashboardSessionResponseDto
{
    public bool IsSessionValid { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
}

public class CheckDashboardSessionRequestHandler : IRequestHandler<CheckDashboardSessionRequestDto, CheckDashboardSessionResponseDto>
{
    private readonly ILogger<CheckDashboardSessionRequestHandler> _logger;
    private readonly UserDbContext _userDbContext;
    private readonly AppSettingsDbContext _appSettingsDbContext;

    public CheckDashboardSessionRequestHandler(
        ILogger<CheckDashboardSessionRequestHandler> logger,
        UserDbContext userDbContext,
        AppSettingsDbContext appSettingsDbContext
    )
    {
        _logger = logger;
        _userDbContext = userDbContext;
        _appSettingsDbContext = appSettingsDbContext;
    }

    public async Task<CheckDashboardSessionResponseDto> Handle(CheckDashboardSessionRequestDto request, CancellationToken cancellationToken)
    {
        CheckDashboardSessionResponseDto responseDto = new();

        #region Check Dashboard Session

        var dashboardSession = await _userDbContext.DashboardSessions
            .Where(
                session =>
                    session.SessionId == request.SessionId &&
                    session.TelegramUserId == request.UserId
            )
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        responseDto.IsSessionValid = dashboardSession != null;

        if (dashboardSession == null)
            return responseDto;

        #endregion

        #region Get User Role

        var checkSudo = await _appSettingsDbContext.Sudoers
            .FirstOrDefaultAsync(sudoer => sudoer.UserId == request.UserId, cancellationToken: cancellationToken);

        if (checkSudo != null)
        {
            responseDto.RoleId = 1;
            responseDto.RoleName = "Sudo";
        }

        #endregion

        _logger.LogDebug("Session {SessionId} for user {UserId} is? {@Session}", request.SessionId, request.UserId, dashboardSession);

        return responseDto;
    }
}