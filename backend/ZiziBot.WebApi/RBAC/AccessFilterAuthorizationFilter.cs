using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZiziBot.Application.Facades;

namespace ZiziBot.WebApi.RBAC;

public class AccessFilterAuthorizationFilter(
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

        var userId = context.HttpContext.Request.Headers.GetUserId();
        var bearerToken = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        if (needAuthenticated)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                context.Result = new UnauthorizedObjectResult(new ApiResponseBase<object>() {
                    Message = "Session invalid, please login to continue"
                });

                return;
            }

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

        switch (roleLevel)
        {
            case RoleLevel.ChatAdminOrPrivate:
                if (!await CheckListChatId(userId))
                {
                    response.StatusCode = HttpStatusCode.Forbidden;
                    response.Message = "You are not authorized to access this resource.";
                }

                break;
            case RoleLevel.User:
                break;
            case RoleLevel.ChatAdmin:
                break;
            case RoleLevel.Sudo:
                break;
        }

        if (response.StatusCode == 0) return;

        context.Result = new JsonResult(response) {
            StatusCode = (int?)response.StatusCode
        };
    }

    private async Task<bool> CheckListChatId(long userId)
    {
        var chatAdmin = await dataFacade.Group.GetChatAdminByUserId(userId);

        return chatAdmin.Any();
    }
}