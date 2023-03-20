using System.Net;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class CheckDashboardBearerSessionRequestDto : ApiRequestBase<CheckDashboardBearerSessionResponseDto>
{
}

public class CheckDashboardBearerSessionResponseDto
{
    public bool IsSessionValid { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public List<UserPermission> Permissions { get; set; }
}

public class UserPermission
{
    public long ChatId { get; set; }
    public string ChatTitle { get; set; }
}

public class CheckDashboardBearerSessionRequestHandler : IRequestHandler<CheckDashboardBearerSessionRequestDto, ApiResponseBase<CheckDashboardBearerSessionResponseDto>>
{
    private readonly ILogger<CheckDashboardSessionRequestHandler> _logger;
    private readonly UserDbContext _userDbContext;
    private readonly ChatDbContext _chatDbContext;
    private readonly AppSettingsDbContext _appSettingsDbContext;

    public CheckDashboardBearerSessionRequestHandler(
        ILogger<CheckDashboardSessionRequestHandler> logger,
        AppSettingsDbContext appSettingsDbContext,
        ChatDbContext chatDbContext,
        UserDbContext userDbContext
    )
    {
        _logger = logger;
        _appSettingsDbContext = appSettingsDbContext;
        _chatDbContext = chatDbContext;
        _userDbContext = userDbContext;
    }

    public async Task<ApiResponseBase<CheckDashboardBearerSessionResponseDto>> Handle(CheckDashboardBearerSessionRequestDto request, CancellationToken cancellationToken)
    {
        ApiResponseBase<CheckDashboardBearerSessionResponseDto> responseDto = new();

        #region Check Dashboard Session

        var dashboardSession = await _userDbContext.DashboardSessions
            .Where(entity =>
                entity.BearerToken == request.BearerToken &&
                entity.Status == (int)EventStatus.Complete
            )
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (dashboardSession == null)
        {
            responseDto.StatusCode = HttpStatusCode.Unauthorized;
            responseDto.Message = "Session not found";

            return responseDto;
        }

        var userId = dashboardSession.TelegramUserId;

        #endregion

        #region Get User Role

        responseDto.Result = new CheckDashboardBearerSessionResponseDto()
        {
            IsSessionValid = true,
            RoleId = 2,
            RoleName = "User",
            Permissions = new List<UserPermission>()
        };

        var checkSudo = await _appSettingsDbContext.Sudoers
            .FirstOrDefaultAsync(entity =>
                    entity.UserId == userId &&
                    entity.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken);

        if (checkSudo != null)
        {
            responseDto.Result.RoleId = 1;
            responseDto.Result.RoleName = "Sudo";
        }

        #endregion

        #region User Permission

        var chatAdmin = await _chatDbContext.ChatAdmin
            .Where(entity =>
                entity.UserId == userId &&
                entity.Status == (int)EventStatus.Complete
            )
            .ToListAsync(cancellationToken: cancellationToken);

        var listPermission = chatAdmin.Select(entity => new UserPermission()
            {
                ChatId = entity.ChatId
            })
            .ToList();

        responseDto.Result.Permissions = listPermission;

        #endregion

        _logger.LogDebug("Session {SessionId} for user {UserId} is? {@Session}", dashboardSession.SessionId, userId, dashboardSession);

        responseDto.Message = "Session is valid";
        responseDto.StatusCode = HttpStatusCode.OK;

        return responseDto;
    }
}