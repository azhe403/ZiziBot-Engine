using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class CheckDashboardSessionRequestDto : ApiRequestBase<CheckDashboardSessionResponseDto>
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

public class CheckDashboardSessionRequestHandler : IRequestHandler<CheckDashboardSessionRequestDto, ApiResponseBase<CheckDashboardSessionResponseDto>>
{
    private readonly ILogger<CheckDashboardSessionRequestHandler> _logger;
    private readonly MongoDbContextBase _mongoDbContext;

    public CheckDashboardSessionRequestHandler(
        ILogger<CheckDashboardSessionRequestHandler> logger,
        MongoDbContextBase mongoDbContext
    )
    {
        _logger = logger;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<ApiResponseBase<CheckDashboardSessionResponseDto>> Handle(CheckDashboardSessionRequestDto request, CancellationToken cancellationToken)
    {
        ApiResponseBase<CheckDashboardSessionResponseDto> responseDto = new()
        {
            Result = new CheckDashboardSessionResponseDto()
        };

        #region Check Dashboard Session
        var dashboardSession = await _mongoDbContext.DashboardSessions
            .Where(
                session =>
                    session.SessionId == request.SessionId &&
                    session.TelegramUserId == request.UserId
            )
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        responseDto.Result.IsSessionValid = dashboardSession != null;

        if (dashboardSession == null)
            return responseDto;
        #endregion

        #region Get User Role
        var checkSudo = await _mongoDbContext.Sudoers
            .FirstOrDefaultAsync(sudoer => sudoer.UserId == request.UserId, cancellationToken: cancellationToken);

        if (checkSudo != null)
        {
            responseDto.Result.RoleId = 1;
            responseDto.Result.RoleName = "Sudo";
        }
        #endregion

        _logger.LogDebug("Session {SessionId} for user {UserId} is? {@Session}", request.SessionId, request.UserId, dashboardSession);

        return responseDto;
    }
}