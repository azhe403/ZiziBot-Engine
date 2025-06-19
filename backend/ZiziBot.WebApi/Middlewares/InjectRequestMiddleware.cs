using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ZiziBot.Application.Facades;

namespace ZiziBot.WebApi.Middlewares;

public class InjectRequestMiddleware(DataFacade dataFacade) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var userRoles = new List<RoleLevel>();
        var bearerToken = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        var ignorePaths = new[] {
            "/api/webhook",
            "/api/logging"
        };

        if (ignorePaths.Any(s => context.Request.Path.Value?.StartsWith(s) == true))
        {
            await next(context);
            return;
        }

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            #region Check Dashboard Session
            var dashboardSession = await dataFacade.MongoEf.DashboardSessions.AsNoTracking()
                .Where(x => x.Status == EventStatus.Complete)
                .Where(x => x.BearerToken == bearerToken)
                .FirstOrDefaultAsync();

            if (dashboardSession == null)
            {
                await context.Response.WriteAsJsonAsync(new ApiResponseBase<object>() {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "Access token is not valid"
                });

                return;
            }

            context.Items.TryAdd(RequestKey.UserId, dashboardSession.TelegramUserId.ToString());
            #endregion

            #region Add List ChatId
            var chatAdmin = await dataFacade.MongoEf.ChatAdmin.AsNoTracking()
                .Where(x => x.Status == EventStatus.Complete)
                .Where(x => x.UserId == dashboardSession.TelegramUserId)
                .ToListAsync();

            var chatIds = chatAdmin.Select(y => y.ChatId).Distinct();

            context.Items.TryAdd(RequestKey.ListChatId, chatIds.ToJson());
            #endregion

            #region Add User Role
            var checkSudo = await dataFacade.MongoEf.Sudoers.AsNoTracking()
                .Where(x => x.Status == EventStatus.Complete)
                .Where(x => x.UserId == dashboardSession.TelegramUserId)
                .FirstOrDefaultAsync();

            if (checkSudo != null)
            {
                userRoles.Add(RoleLevel.Sudo);
            }

            context.Items.TryAdd(RequestKey.UserRole, userRoles);
            #endregion
        }

        await next(context);
    }
}