using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.WebApi.RBAC;

public class AccessLevelAuthorizationFilter : IAsyncAuthorizationFilter
{
    private ILogger<AccessLevelAuthorizationFilter> _logger;
    private readonly AccessLevelEnum _accessLevelEnum;
    private ChatDbContext _chatDbContext;

    public AccessLevelAuthorizationFilter(AccessLevelEnum accessLevelEnum)
    {
        _accessLevelEnum = accessLevelEnum;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var response = new ApiResponseBase<object>
        {
            transactionId = context.HttpContext.Request.Headers.GetTransactionId()
        };

        var listChatId = context.HttpContext.Request.Headers.GetListChatId();
        var userId = context.HttpContext.Request.Headers.GetUserId();

        _chatDbContext = context.HttpContext.RequestServices.GetRequiredService<ChatDbContext>();
        _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AccessLevelAuthorizationFilter>>();

        switch (_accessLevelEnum)
        {
            case AccessLevelEnum.AdminOrPrivate:
                if (!await GetListChatId(userId))
                {
                    response.StatusCode = HttpStatusCode.Forbidden;
                    response.Message = "You are not authorized to access this resource.";
                }
                break;
            case AccessLevelEnum.User:
                break;
            case AccessLevelEnum.Admin:
                break;
            case AccessLevelEnum.Root:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (response.StatusCode == 0) return;

        context.Result = new JsonResult(response)
        {
            StatusCode = (int?)response.StatusCode
        };
    }

    private async Task<bool> GetListChatId(long userId)
    {
        var chatAdmin = await _chatDbContext.ChatAdmin
            .Where(entity =>
                entity.UserId == userId &&
                entity.Status == (int)EventStatus.Complete
            )
            .ToListAsync();

        return chatAdmin.Any();
    }
}