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
    public string UserName { get; set; }
    public string PhotoUrl { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public List<UserFeature> Features { get; set; }
}

public class UserFeature
{
    public string Title { get; set; }
    public string Url { get; set; }
    public int MinimumRole { get; set; }
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

        var result = new CheckDashboardBearerSessionResponseDto()
        {
            IsSessionValid = true,
            UserName = dashboardSession.FirstName,
            PhotoUrl = dashboardSession.PhotoUrl,
            RoleId = 2,
            RoleName = "User",
            Features = new List<UserFeature>()
        };

        var checkSudo = await _appSettingsDbContext.Sudoers
            .FirstOrDefaultAsync(entity =>
                    entity.UserId == userId &&
                    entity.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken);

        if (checkSudo != null)
        {
            result.RoleId = 1;
            result.RoleName = "Sudo";
        }

        #endregion

        result.Features = GetFeatures(result.RoleId);

        _logger.LogDebug("Session {SessionId} for user {UserId} is? {@Session}", dashboardSession.SessionId, userId, dashboardSession);

        responseDto.Message = "Session is valid";
        responseDto.StatusCode = HttpStatusCode.OK;
        responseDto.Result = result;

        return responseDto;
    }

    private List<UserFeature> GetFeatures(int roleId)
    {
        var features = new List<UserFeature>()
        {
            new()
            {
                Title = "Mirror User",
                Url = "/mirror-user/management",
                MinimumRole = 1
            },
            new()
            {
                Title = "Fed management",
                Url = "/antispam/fed-ban-management",
                MinimumRole = 1
            },
            new()
            {
                Title = "Notes Management",
                Url = "/notes",
                MinimumRole = 1
            },
            new()
            {
                Title = "Angular",
                Url = "/angular",
                MinimumRole = 9
            }
        };

        return features
            .Where(x => x.MinimumRole >= roleId)
            .ToList();
    }
}