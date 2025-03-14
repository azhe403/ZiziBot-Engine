using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZiziBot.DataSource.MongoEf;

namespace ZiziBot.Hangfire;

public class HangfireAuthorizationFilter(
    ILogger<HangfireAuthorizationFilter> logger,
    MongoEfContext mongoDbContext
) : IDashboardAsyncAuthorizationFilter
{
    public async Task<bool> AuthorizeAsync(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        httpContext.Request.Cookies.TryGetValue("bearer_token", out var sessionId);

        logger.LogDebug("Checking Hangfire authorization..");

        if (sessionId != null)
        {
            var checkSession = await CheckSession(sessionId);
            logger.LogDebug("SessionId is have access? {CheckSession}", checkSession);

            if (checkSession)
                return true;
        }

        logger.LogInformation("Session expired or invalid. Redirecting to Home..");
        httpContext.Response.Redirect("/", true);

        return true;
    }

    async Task<bool> CheckSession(string sessionId)
    {
        var dashboardSessions = await mongoDbContext.DashboardSessions.AsNoTracking()
            .Where(entity => entity.BearerToken == sessionId)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (dashboardSessions == null)
            return false;

        var sudoer = mongoDbContext.Sudoers
            .Where(entity => entity.UserId == dashboardSessions.TelegramUserId)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        return sudoer != null;
    }
}