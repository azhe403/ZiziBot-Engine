using System.Linq.Expressions;

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

    public static T? GetCommandParamAt<T>(this string[] strings, int index)
    {
        dynamic value = strings.ElementAtOrDefault(index);

        if (value == null)
            return default;

        return (T)Convert.ChangeType(value, typeof(T));
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> whereClause)
    {
        return condition ? query.Where(whereClause) : query;
    }

    public static bool Contains(this string message, params string[] keywords)
    {
        return keywords.Any(keyword => message.Contains(keyword, StringComparison.InvariantCultureIgnoreCase));
    }
}