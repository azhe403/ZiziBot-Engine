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

    public static WebhookSource GetWebHookSource(this HttpRequest httpRequest)
    {
        var headers = httpRequest.Headers;
        var source = headers switch
        {
            { } when headers.Any(pair => pair.Key.Contains("GitHub", StringComparison.InvariantCultureIgnoreCase)) => WebhookSource.GitHub,
            _ => WebhookSource.Unknown
        };

        return source != WebhookSource.Unknown ? source : WebhookSource.Unknown;
    }

    public static WebhookSource GetWebHookSource(this string userAgent)
    {
        var source = userAgent switch
        {
            { } when userAgent.Contains("GitHub", StringComparison.InvariantCultureIgnoreCase) => WebhookSource.GitHub,
            _ => WebhookSource.Unknown
        };

        return source != WebhookSource.Unknown ? source : WebhookSource.Unknown;
    }
}