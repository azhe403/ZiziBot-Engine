using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Hangfire;

public class HangfireAuthorizationFilter : IDashboardAsyncAuthorizationFilter
{
    private readonly ILogger<HangfireAuthorizationFilter> _logger;
    private readonly MongoDbContextBase _mongoDbContext;

    public HangfireAuthorizationFilter(
        ILogger<HangfireAuthorizationFilter> logger,
        MongoDbContextBase mongoDbContext
    )
    {
        _logger = logger;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<bool> AuthorizeAsync(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        httpContext.Request.Cookies.TryGetValue("bearer_token", out var sessionId);

        _logger.LogDebug("Checking Hangfire authorization..");

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
        var dashboardSessions = await _mongoDbContext.DashboardSessions
            .FirstOrDefaultAsync(session =>
                session.BearerToken == sessionId &&
                session.Status == (int)EventStatus.Complete
            );

        if (dashboardSessions == null)
            return false;

        var sudoer = _mongoDbContext.Sudoers
            .FirstOrDefault(sudo =>
                sudo.UserId == dashboardSessions.TelegramUserId &&
                sudo.Status == (int)EventStatus.Complete
            );

        if (sudoer == null)
            return false;

        return true;
    }
}