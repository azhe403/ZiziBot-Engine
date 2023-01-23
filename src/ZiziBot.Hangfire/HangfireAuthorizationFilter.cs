using Microsoft.Extensions.Logging;

namespace ZiziBot.Hangfire;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly ILogger<HangfireAuthorizationFilter> _logger;
    private readonly UserDbContext _userDbContext;

    public HangfireAuthorizationFilter(ILogger<HangfireAuthorizationFilter> logger, UserDbContext userDbContext)
    {
        _logger = logger;
        _userDbContext = userDbContext;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        httpContext.Request.Cookies.TryGetValue("session_id", out var sessionId);

        _logger.LogInformation("Checking Hangfire sessionId: {SessionId}", sessionId);

        if (sessionId != null)
        {
            return CheckSession(sessionId);
        }

        _logger.LogInformation("Session expired or invalid. Redirecting to Home..");
        httpContext.Response.Redirect("/", true);

        return true;
    }

    private bool CheckSession(string sessionId)
    {
        var dashboardSessions = _userDbContext.DashboardSessions
            .FirstOrDefault(session => session.SessionId == sessionId);

        var isSessionValid = dashboardSessions != null;
        _logger.LogDebug("SessionId {SessionId} is valid? {isSessionValid}", sessionId, isSessionValid);

        return isSessionValid;
    }
}