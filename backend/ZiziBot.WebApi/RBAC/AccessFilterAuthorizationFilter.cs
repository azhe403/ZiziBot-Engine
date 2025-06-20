using System.Net;
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
        var response = new ApiResponseBase<object> {
            TransactionId = context.HttpContext.GetTransactionId()
        };

        logger.LogDebug("Access '{Flag}' role level minimum: {Role}", flag, roleLevel);

        if (roleLevel == RoleLevel.None)
            return;

        var bearerToken = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "").Trim();
        var items = context.HttpContext.Items;

        var session = await dataFacade.MongoEf.DashboardSessions
            .Where(x => x.BearerToken == bearerToken)
            .Where(x => x.Status == EventStatus.Complete)
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefaultAsync();

        if (session == null)
        {
            context.Result = new UnauthorizedObjectResult(new ApiResponseBase<object>() {
                Message = "Access token is invalid."
            });

            return;
        }

        var roles = items[RequestKey.UserRole] as List<RoleLevel>;

        if (roles?.Contains(roleLevel) != true)
        {
            response.StatusCode = HttpStatusCode.Forbidden;
            response.Message = "You are not authorized to access this resource.";
        }

        if (response.StatusCode == 0) return;

        context.Result = new JsonResult(response) {
            StatusCode = (int?)response.StatusCode
        };
    }
}