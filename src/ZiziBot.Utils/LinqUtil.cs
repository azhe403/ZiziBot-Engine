namespace ZiziBot.Utils;

public static class LinqUtil
{
    public static T RandomPick<T>(this List<T> source)
    {
        var random = new Random();
        var index = random.Next(source.Count);
        var item = source.ElementAt(index);

        return item;
    }

    public static IEnumerable<string> TrimEach(this IEnumerable<string> source)
    {
        return source.Select(x => x.Trim()).ToList();
    }

    public static T GetCommandParamAt<T>(this string[] strings, int index)
    {
        dynamic value = strings.ElementAtOrDefault(index) ?? string.Empty;

        return (T) Convert.ChangeType(value, typeof(T));
    }
}