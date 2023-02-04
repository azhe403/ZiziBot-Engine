namespace ZiziBot.Utils;

public static class StringUtil
{
    public static T GetCommandParamAt<T>(this string param, int index, string separator = " ")
    {
        return param.Split(separator, StringSplitOptions.TrimEntries).GetCommandParamAt<T>(index);
    }
}