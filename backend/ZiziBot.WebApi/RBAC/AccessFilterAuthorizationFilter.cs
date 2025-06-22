using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZiziBot.Application.Facades;

namespace ZiziBot.WebApi.RBAC;

public class AccessFilterAuthorizationFilter(
    string flag,
    RoleLevel roleLevel,
    ILogger<AccessFilterAuthorizationFilter> logger,
    DataFacade dataFacade
) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userRoles = new List<RoleLevel>();
        var response = new ApiResponseBase<object> {
            TransactionId = context.HttpContext.GetTransactionId()
        };

        logger.LogDebug("Access '{Flag}' role level minimum: {Role}", flag, roleLevel);

        if (roleLevel == RoleLevel.None) // allow anonymous
            return;

        var bearerToken = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "").Trim();

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            #region Check Dashboard Session
            var dashboardSession = await dataFacade.MongoEf.DashboardSessions.AsNoTracking()
                .Where(x => x.BearerToken == bearerToken)
                .Where(x => x.Status == EventStatus.Complete)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();

            if (dashboardSession == null)
            {
                response.Unauthorized("Access token is not valid");
                context.Result = new UnauthorizedObjectResult(response);
                return;
            }

            userRoles.Add(RoleLevel.User);
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

            context.HttpContext.Items.TryAdd("UserRoles", userRoles.ToArray());
            #endregion
        }

        if (!userRoles.Contains(roleLevel))
        {
            response.Unauthorized("You are not authorized to access this resource");
            context.Result = new UnauthorizedObjectResult(response);
        }
    }
}