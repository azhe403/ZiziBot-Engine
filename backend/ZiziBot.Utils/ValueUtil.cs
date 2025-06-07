using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Ardalis.GuardClauses;
using Humanizer;
using ZiziBot.Contracts.Enums;

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

    public static Dictionary<string, object> ToDictionary(this object? values, StringType stringType = StringType.Original)
    {
        var symbols = new Dictionary<string, object>();

        if (values != null)
        {
            var properties = values.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(values, null) ?? string.Empty;
                var propertyName = property.Name;
                propertyName = stringType switch {
                    StringType.Original => propertyName,
                    StringType.SnakeCase => propertyName.Underscore().ToLower(),
                    StringType.PascalCase => propertyName.Pascalize(),
                    StringType.TitleCase => propertyName.Titleize(),
                    _ => propertyName
                };

                symbols.Add(propertyName, value.ToString() ?? string.Empty);
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

    public static string EnsureNotNullOrWhiteSpace([NotNull] [ValidatedNotNull] this string? obj)
    {
        return Guard.Against.NullOrWhiteSpace(obj);
    }

    public static T EnsureNotNull<T>([NotNull] [ValidatedNotNull] this T? obj) where T : class
    {
        return Guard.Against.Null(obj);
    }
}