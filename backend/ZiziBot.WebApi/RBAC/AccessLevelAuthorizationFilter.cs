using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ZiziBot.WebApi.RBAC;

public class AccessLevelAuthorizationFilter : IAsyncAuthorizationFilter
{
    private ILogger<AccessLevelAuthorizationFilter> _logger;
    private readonly ApiRoleLevel _apiRoleLevel;
    private GroupRepository _groupRepository;

    public AccessLevelAuthorizationFilter(ApiRoleLevel apiRoleLevel)
    {
        _apiRoleLevel = apiRoleLevel;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var response = new ApiResponseBase<object>
        {
            transactionId = context.HttpContext.Request.Headers.GetTransactionId()
        };

        var userId = context.HttpContext.Request.Headers.GetUserId();

        context.HttpContext.RequestServices.GetRequiredService<ChatDbContext>();
        _groupRepository = context.HttpContext.RequestServices.GetRequiredService<GroupRepository>();
        _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AccessLevelAuthorizationFilter>>();

        switch (_apiRoleLevel)
        {
            case ApiRoleLevel.AdminOrPrivate:
                if (!await CheckListChatId(userId))
                {
                    response.StatusCode = HttpStatusCode.Forbidden;
                    response.Message = "You are not authorized to access this resource.";
                }
                break;
            case ApiRoleLevel.User:
                break;
            case ApiRoleLevel.Admin:
                break;
            case ApiRoleLevel.Root:
                break;
        }

        if (response.StatusCode == 0) return;

        context.Result = new JsonResult(response)
        {
            StatusCode = (int?)response.StatusCode
        };
    }

    private async Task<bool> CheckListChatId(long userId)
    {
        var chatAdmin = await _groupRepository.GetChatAdminByUserId(userId);

        return chatAdmin.Any();
    }
}