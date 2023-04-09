using System.Web;

namespace ZiziBot.Utils;

public static class StringUtil
{
    public static T? GetCommandParamAt<T>(this string? param, int index, string separator = " ")
    {
        return string.IsNullOrEmpty(param) ? default : param.Split(separator, StringSplitOptions.TrimEntries).GetCommandParamAt<T>(index);
    }

    public static bool IsCommand(this string? text, string command)
    {
        return text.GetCommandParamAt<string>(0)?.ToLower().Equals(command.ToLower()) ?? false;
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
        return Nanoid.Nanoid.Generate(size: size);
    }

    public static async Task<string> GetNanoIdAsync(int size = 11)
    {
        return await Nanoid.Nanoid.GenerateAsync(size: size);
    }

    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static TValue ToEnum<TValue>(this string value, TValue defaultValue) where TValue : struct
    {
        if (string.IsNullOrEmpty(value)) return defaultValue;

        return Enum.TryParse<TValue>(value, true, out var result) ? result : defaultValue;
    }
}