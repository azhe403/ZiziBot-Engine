using Microsoft.AspNetCore.Http;

namespace ZiziBot.Application.Utils;

public static class HttpContextUtil
{
    public static string GetTransactionId(this IHttpContextAccessor contextAccessor)
    {
        var transactionId = contextAccessor.HttpContext?.Request.Headers["transactionId"].ToString();
        return transactionId ?? string.Empty;
    }

    public static long GetUserId(this IHttpContextAccessor contextAccessor)
    {
        var userId = Convert.ToInt64(contextAccessor.HttpContext?.Request.Headers["userId"]);
        return userId;
    }
}