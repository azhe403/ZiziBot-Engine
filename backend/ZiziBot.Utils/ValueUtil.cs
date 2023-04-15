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

    public static Dictionary<string, string> ToDictionary(this object? values)
    {
        var symbols = new Dictionary<string, string>();

        if (values != null)
        {
            var properties = values.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(values, null) ?? string.Empty;
                symbols.Add(property.Name.Underscore(), value.ToString() ?? string.Empty);
            }
        }

        return symbols;
    }
}