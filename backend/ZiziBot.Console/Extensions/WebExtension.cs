using System.Collections.Specialized;
using System.Web;
using ZiziBot.Common.Utils;

namespace ZiziBot.Console.Extensions;

public static class WebExtension
{
    // get entire querystring name/value collection
    public static NameValueCollection QueryString(this NavigationManager navigationManager)
    {
        return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
    }

    // get single querystring value with specified key
    public static string QueryString(this NavigationManager navigationManager, string key)
    {
        return navigationManager.QueryString()[key];
    }

    // get query string as T
    public static Dictionary<string, string> QueryStringKv(this NavigationManager navigationManager)
    {
        var queryString = navigationManager.QueryString();
        var queryStringKV = queryString.AllKeys.ToDictionary(key => key, key => queryString[key]);
        return queryStringKV;
    }

    // get query string as T
    public static T? QueryString<T>(this NavigationManager navigationManager)
    {
        return navigationManager.QueryStringKv().ToJson().ToObject<T>();
    }
}