using Microsoft.AspNetCore.Http;
using MongoFramework.Linq;

namespace ZiziBot.WebApi.Middlewares;

public class InjectHeaderMiddleware : IMiddleware
{
    private readonly ChatDbContext _chatDbContext;
    private readonly UserDbContext _userDbContext;

    public InjectHeaderMiddleware(ChatDbContext chatDbContext, UserDbContext userDbContext)
    {
        _chatDbContext = chatDbContext;
        _userDbContext = userDbContext;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var bearerToken = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(bearerToken))
        {
            await next(context);
        }

        #region Check Dashboard Session

        var dashboardSession = await _userDbContext.DashboardSessions
            .Where(entity =>
                entity.BearerToken == bearerToken &&
                entity.Status == (int)EventStatus.Complete
            )
            .FirstOrDefaultAsync();

        if (dashboardSession != null)
        {
            context.Request.Headers.Add(HeaderKey.UserId, dashboardSession.TelegramUserId.ToString());
        }

        #endregion

        #region Add List ChatId

        if (dashboardSession != null)
        {
            var chatAdmin = await _chatDbContext.ChatAdmin
                .Where(entity =>
                    entity.UserId == dashboardSession.TelegramUserId &&
                    entity.Status == (int)EventStatus.Complete
                )
                .ToListAsync();

            var chatIds = chatAdmin.Select(y => y.ChatId).Distinct();

            context.Request.Headers.Add(HeaderKey.ListChatId, chatIds.ToJson());
        }

        #endregion

        await next(context);

    }
}