using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ZiziBot.WebApi.RBAC;

public class AccessFilterAuthorizationFilter : IAsyncAuthorizationFilter
{
    private ILogger<AccessFilterAuthorizationFilter> _logger;
    private readonly RoleLevel _roleLevel;
    private readonly bool _checkHeader;
    private GroupRepository _groupRepository;

    public AccessFilterAuthorizationFilter(RoleLevel roleLevel, bool checkHeader, ILogger<AccessFilterAuthorizationFilter> logger, GroupRepository groupRepository)
    {
        _roleLevel = roleLevel;
        _checkHeader = checkHeader;
        _logger = logger;
        _groupRepository = groupRepository;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var response = new ApiResponseBase<object>
        {
            TransactionId = context.HttpContext.Request.Headers.GetTransactionId()
        };

        #region Check Header: TransactionId
        if (context.HttpContext.Request.Headers.GetTransactionId().IsNullOrEmpty() && _checkHeader)
        {
            response.BadRequest("Please set 'transactionId' in Headers");
        }
        #endregion

        var userId = context.HttpContext.Request.Headers.GetUserId();

        context.HttpContext.RequestServices.GetRequiredService<ChatDbContext>();
        _groupRepository = context.HttpContext.RequestServices.GetRequiredService<GroupRepository>();
        _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AccessFilterAuthorizationFilter>>();

        switch (_roleLevel)
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