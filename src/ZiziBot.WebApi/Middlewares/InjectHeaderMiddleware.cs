using System.Net;
using Microsoft.AspNetCore.Http;
using MongoFramework.Linq;

namespace ZiziBot.WebApi.Middlewares;

public class InjectHeaderMiddleware : IMiddleware
{
    private readonly AppSettingsDbContext _appSettingsDbContext;
    private readonly ChatDbContext _chatDbContext;
    private readonly UserDbContext _userDbContext;

    public InjectHeaderMiddleware(AppSettingsDbContext appSettingsDbContext, ChatDbContext chatDbContext, UserDbContext userDbContext)
    {
        _appSettingsDbContext = appSettingsDbContext;
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

        if (dashboardSession == null)
        {
            await context.Response.WriteAsJsonAsync(new ApiResponseBase<object>()
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Message = "Unauthorized"
            });

            return;
        }

        context.Request.Headers.Add(HeaderKey.UserId, dashboardSession.TelegramUserId.ToString());

        #endregion

        #region Add List ChatId

        var chatAdmin = await _chatDbContext.ChatAdmin
            .Where(entity =>
                entity.UserId == dashboardSession.TelegramUserId &&
                entity.Status == (int)EventStatus.Complete
            )
            .ToListAsync();

        var chatIds = chatAdmin.Select(y => y.ChatId).Distinct();

        context.Request.Headers.Add(HeaderKey.ListChatId, chatIds.ToJson());

        #endregion

        #region Add User Role

        var userRole = ApiRole.Guest;

        var checkSudo = await _appSettingsDbContext.Sudoers
            .FirstOrDefaultAsync(entity =>
                entity.UserId == dashboardSession.TelegramUserId &&
                entity.Status == (int)EventStatus.Complete);

        if (checkSudo != null)
        {
            userRole = ApiRole.Sudo;
        }

        context.Request.Headers.Add(HeaderKey.UserRole, userRole.ToString());

        #endregion

        await next(context);
    }
}