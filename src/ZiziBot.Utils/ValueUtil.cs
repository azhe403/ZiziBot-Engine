using System.ComponentModel;

namespace ZiziBot.Utils;

public static class ValueUtil
{
    public static T Convert<T>(this object? input, T defaultVal)
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (converter != null)
        {
            return (T)converter.ConvertFrom(input);
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
}