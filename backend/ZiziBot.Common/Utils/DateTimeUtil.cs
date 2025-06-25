using TimeSpanParserUtil;

namespace ZiziBot.Common.Utils;

public static class DateTimeUtil
{
    public static TimeSpan ToTimeSpan(this string timeSpanStr)
    {
        return TimeSpanParser.Parse(timeSpanStr);
    }
}