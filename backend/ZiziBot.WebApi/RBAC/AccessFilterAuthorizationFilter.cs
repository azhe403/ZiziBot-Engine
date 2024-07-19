using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ZiziBot.WebApi.RBAC;

public class AccessFilterAuthorizationFilter(
    RoleLevel roleLevel,
    bool checkHeader,
    ILogger<AccessFilterAuthorizationFilter> logger,
    GroupRepository groupRepository
) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var response = new ApiResponseBase<object> {
            TransactionId = context.HttpContext.Request.Headers.GetTransactionId()
        };

        var userId = context.HttpContext.Request.Headers.GetUserId();

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
        var chatAdmin = await groupRepository.GetChatAdminByUserId(userId);

        return chatAdmin.Any();
    }
}