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
}