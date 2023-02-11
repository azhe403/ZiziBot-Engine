using System.Net;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class CheckDashboardSessionIdRequestDto : ApiRequestBase<CheckDashboardSessionIdResponseDto>
{
    public string SessionId { get; set; }
}

public class CheckDashboardSessionIdResponseDto
{
    public bool IsSessionValid { get; set; }
    public long UserId { get; set; }
    public string Role { get; set; }
}

public class CheckDashboardSessionIdRequestHandler : IRequestHandler<CheckDashboardSessionIdRequestDto, ApiResponseBase<CheckDashboardSessionIdResponseDto>>
{
    private readonly ILogger<CheckDashboardSessionRequestHandler> _logger;
    private readonly UserDbContext _userDbContext;
    private readonly AppSettingsDbContext _appSettingsDbContext;

    public CheckDashboardSessionIdRequestHandler(
        ILogger<CheckDashboardSessionRequestHandler> logger,
        UserDbContext userDbContext,
        AppSettingsDbContext appSettingsDbContext
    )
    {
        _logger = logger;
        _userDbContext = userDbContext;
        _appSettingsDbContext = appSettingsDbContext;
    }

    public async Task<ApiResponseBase<CheckDashboardSessionIdResponseDto>> Handle(CheckDashboardSessionIdRequestDto request, CancellationToken cancellationToken)
    {
        ApiResponseBase<CheckDashboardSessionIdResponseDto> apiResponse = new();

        #region Check Dashboard Session

        var dashboardSession = await _userDbContext.DashboardSessions
            .Where(
                session =>
                    session.SessionId == request.SessionId
            )
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        apiResponse.Result.IsSessionValid = dashboardSession != null;

        if (dashboardSession == null)
        {
            apiResponse.StatusCode = HttpStatusCode.OK;
            return apiResponse;
        }

        apiResponse.Result.UserId = dashboardSession.TelegramUserId;

        #endregion

        #region Get User Role

        var checkSudo = await _appSettingsDbContext.Sudoers
            .FirstOrDefaultAsync(
                sudoer =>
                    sudoer.UserId == dashboardSession.TelegramUserId,
                cancellationToken: cancellationToken
            );

        if (checkSudo != null)
        {
            apiResponse.Result.Role = "Sudo";
        }

        #endregion

        _logger.LogDebug("SessionId {SessionId} for user {UserId} is? {@Session}", request.SessionId, dashboardSession.TelegramUserId, dashboardSession);

        apiResponse.StatusCode = HttpStatusCode.OK;
        return apiResponse;
    }
}