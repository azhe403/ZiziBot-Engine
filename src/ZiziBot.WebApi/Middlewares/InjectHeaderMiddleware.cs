using Microsoft.AspNetCore.Http;
using MongoFramework.Linq;

namespace ZiziBot.WebApi.Middlewares;

public class InjectHeaderMiddleware : IMiddleware
{
    private readonly UserDbContext _userDbContext;

    public InjectHeaderMiddleware(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var bearerToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        #region Check Dashboard Session

        var dashboardSession = await _userDbContext.DashboardSessions
            .Where(entity =>
                entity.BearerToken == bearerToken &&
                entity.Status == (int)EventStatus.Complete
            )
            .FirstOrDefaultAsync();

        if (dashboardSession != null)
        {
            context.Request.Headers.Add("userId", dashboardSession.TelegramUserId.ToString());
        }

        await next(context);

        #endregion
    }
}