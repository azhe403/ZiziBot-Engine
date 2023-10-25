using Flurl;

namespace ZiziBot.Utils;

public static class UrlUtil
{
    public static bool IsValidUrl(this string? urlPath)
    {
        return Url.IsValid(urlPath);
    }

    public static string? UrlSegment(this string url, int index, string? defaultValue = null)
    {
        return Url.Parse(url).PathSegments.ElementAtOrDefault(index) ?? defaultValue;
    }
}