using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Hangfire;

public class HangfireAuthorizationFilter : IDashboardAsyncAuthorizationFilter
{
    private readonly ILogger<HangfireAuthorizationFilter> _logger;
    private readonly AppSettingsDbContext _appSettingsDbContext;
    private readonly UserDbContext _userDbContext;

    public HangfireAuthorizationFilter(
        ILogger<HangfireAuthorizationFilter> logger,
        AppSettingsDbContext appSettingsDbContext,
        UserDbContext userDbContext
    )
    {
        _logger = logger;
        _appSettingsDbContext = appSettingsDbContext;
        _userDbContext = userDbContext;
    }

    public async Task<bool> AuthorizeAsync(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        httpContext.Request.Cookies.TryGetValue("bearer_token", out var sessionId);

        _logger.LogInformation("Checking Hangfire sessionId: {SessionId}", sessionId);

        if (sessionId != null)
        {
            var checkSession = await CheckSession(sessionId);
            _logger.LogDebug("SessionId is have access? {CheckSession}", checkSession);
            if (checkSession)
                return true;
        }

        _logger.LogInformation("Session expired or invalid. Redirecting to Home..");
        httpContext.Response.Redirect("/", true);

        return true;
    }

    private async Task<bool> CheckSession(string sessionId)
    {
        var dashboardSessions = await _userDbContext.DashboardSessions
            .FirstOrDefaultAsync(session =>
                session.BearerToken == sessionId &&
                session.Status == (int)EventStatus.Complete
            );

        if (dashboardSessions == null)
            return false;

        var sudoer = _appSettingsDbContext.Sudoers
            .FirstOrDefault(sudo =>
                sudo.UserId == dashboardSessions.TelegramUserId &&
                sudo.Status == (int)EventStatus.Complete
            );

        if (sudoer == null)
            return false;

        return true;
    }
}