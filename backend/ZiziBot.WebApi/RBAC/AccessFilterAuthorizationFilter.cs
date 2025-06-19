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
    bool checkHeader,
    bool needAuthenticated,
    ILogger<AccessFilterAuthorizationFilter> logger,
    GroupRepository groupRepository,
    DataFacade dataFacade
) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var response = new ApiResponseBase<object> {
            TransactionId = context.HttpContext.GetTransactionId()
        };

        var bearerToken = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var items = context.HttpContext.Items;

        if (needAuthenticated)
        {
            var session = await dataFacade.MongoEf.DashboardSessions
                .Where(x => x.BearerToken == bearerToken)
                .Where(x => x.Status == EventStatus.Complete)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();

            if (session == null)
            {
                context.Result = new UnauthorizedObjectResult(new ApiResponseBase<object>() {
                    Message = "Session invalid, please login to continue"
                });

                return;
            }
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