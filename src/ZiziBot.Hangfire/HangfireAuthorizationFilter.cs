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
        var sessionId = context.Request.GetQuery("session_id");
        var httpContext = context.GetHttpContext();
        httpContext.Request.Cookies.TryGetValue("hangfire_session", out var hangfireSessionId);

        var currentSessionId = sessionId ?? hangfireSessionId;

        _logger.LogInformation("Checking Hangfire sessionId: {SessionId}", currentSessionId);

        if (sessionId != null)
        {
            _logger.LogDebug("Updating Hangfire sessionId");
            httpContext.Response.Headers.Add("Set-Cookie", $"hangfire_session={sessionId}");

            return CheckSession(sessionId);
        }

        if (hangfireSessionId != null)
        {
            return CheckSession(hangfireSessionId);
        }

        // httpContext.Response.WriteAsync("<script>alert('Your session is expired!');</script>;window.location ='/';");
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