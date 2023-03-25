using System.Web;

namespace ZiziBot.Utils;

public static class StringUtil
{
    public static T? GetCommandParamAt<T>(this string? param, int index, string separator = " ")
    {
        return string.IsNullOrEmpty(param) ? default : param.Split(separator, StringSplitOptions.TrimEntries).GetCommandParamAt<T>(index);
    }

    public static string HtmlEncode(this string html)
    {
        return HttpUtility.HtmlEncode(html);
    }

    public static string HtmlDecode(this string html)
    {
        return HttpUtility.HtmlDecode(html);
    }

    public static string GetNanoId(int size = 11)
    {
        var id = Nanoid.Nanoid.Generate(size: size);
        return id;
    }

    public static async Task<string> GetNanoIdAsync(int size = 11)
    {
        var id = await Nanoid.Nanoid.GenerateAsync(size: size);
        return id;
    }

    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }
}