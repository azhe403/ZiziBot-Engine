using Flurl;

namespace ZiziBot.Utils;

public static class UrlUtil
{
    public static bool IsValidUrl(this string? urlPath)
    {
        return Url.IsValid(urlPath);
    }
}