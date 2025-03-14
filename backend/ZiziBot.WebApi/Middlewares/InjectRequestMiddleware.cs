using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ZiziBot.Application.Facades;

namespace ZiziBot.WebApi.Middlewares;

public class InjectRequestMiddleware(DataFacade dataFacade) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var bearerToken = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(bearerToken))
        {
            await next(context);
            return;
        }

        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        var ignorePaths = new[] {
            "/api/webhook",
            "/api/logging"
        };

        if (ignorePaths.Any(s => context.Request.Path.Value.StartsWith(s)))
        {
            await next(context);
            return;
        }

        #region Check Dashboard Session
        var dashboardSession = await dataFacade.MongoEf.DashboardSessions
            .Where(entity =>
                entity.BearerToken == bearerToken &&
                entity.Status == EventStatus.Complete
            )
            .FirstOrDefaultAsync();

        if (dashboardSession == null)
        {
            await context.Response.WriteAsJsonAsync(new ApiResponseBase<object>() {
                StatusCode = HttpStatusCode.Unauthorized,
                Message = "Unauthorized"
            });

            return;
        }

        context.Items.TryAdd(RequestKey.UserId, dashboardSession.TelegramUserId.ToString());
        #endregion

        #region Add List ChatId
        var chatAdmin = await dataFacade.MongoEf.ChatAdmin
            .Where(entity =>
                entity.UserId == dashboardSession.TelegramUserId &&
                entity.Status == EventStatus.Complete
            )
            .ToListAsync();

        var chatIds = chatAdmin.Select(y => y.ChatId).Distinct();

        context.Items.TryAdd(RequestKey.ListChatId, chatIds.ToJson());
        #endregion

        #region Add User Role
        var userRole = ApiRole.Guest;

        var checkSudo = await dataFacade.MongoEf.Sudoers
            .FirstOrDefaultAsync(entity =>
                entity.UserId == dashboardSession.TelegramUserId &&
                entity.Status == EventStatus.Complete);

        if (checkSudo != null)
        {
            userRole = ApiRole.Sudo;
        }

        context.Items.TryAdd(RequestKey.UserRole, userRole.ToString());
        #endregion

        await next(context);
    }
}