using System.ComponentModel;
using Humanizer;

namespace ZiziBot.Utils;

public static class ValueUtil
{
    public static T Convert<T>(this object? input, T defaultVal)
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (converter != null)
        {
            return (T)converter.ConvertFromString(input.ToString());
        }
        return defaultVal;
    }

    public static T Convert<T>(this object? input)
    {
        return Convert(input, default(T));
    }


    public static TNumber Convert<TNumber>(this string? src)
    {
        if (src == null) return default;

        return Convert<TNumber>((object?)src?.Replace(".", ""));
    }

    public static bool TryConvert<T>(this object? input, T defaultVal, out T result)
    {
        try
        {
            result = Convert(input, defaultVal);
            return true;
        }
        catch (Exception exception)
        {
            result = defaultVal;
            return input == null;
        }
    }

    public static Dictionary<string, string> ToDictionary(this object? values, LetterCasing letterCasing = LetterCasing.LowerCase)
    {
        var symbols = new Dictionary<string, string>();

        if (values != null)
        {
            var properties = values.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(values, null) ?? string.Empty;
                symbols.Add(property.Name.Underscore().Humanize(letterCasing), value.ToString() ?? string.Empty);
            }
        }

        return symbols;
    }

    public static bool IsNull<T, TU>(this KeyValuePair<T, TU> pair)
    {
        return pair.Equals(new KeyValuePair<T, TU>());
    }

    public static TDestination FromDictionary<TDestination>(this IDictionary<string, string> dictionary) where TDestination : new()
    {
        var obj = new TDestination();

        foreach (var propertyInfo in typeof(TDestination).GetProperties())
        {
            propertyInfo.SetValue(obj, dictionary[propertyInfo.Name]);
        }

        return obj;
    }
}