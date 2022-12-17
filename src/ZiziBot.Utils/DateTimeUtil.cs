using TimeSpanParserUtil;

namespace ZiziBot.Utils;

public static class DateTimeUtil
{
    public static TimeSpan ToTimeSpan(this string timeSpanStr)
    {
        return TimeSpanParser.Parse(timeSpanStr);
    }
}