using System.Globalization;
using Humanizer;
using Humanizer.Localisation;
using TimeSpanParserUtil;

namespace ZiziBot.Utils;

public static class TimeUtil
{
    public static string GetDelay(this DateTime dateTime)
    {
        var dateNow = DateTime.UtcNow;
        var span = dateNow - dateTime;
        var delay = span.ToString(@"s\,fff");
        return delay;
    }

    public static TimeSpan ParseDateTime(this string dateTime)
    {
        return TimeSpanParser.Parse(dateTime);
    }

    public static string ForHuman(this TimeSpan timeSpan, int precision = 10)
    {
        return timeSpan.Humanize(precision: precision, maxUnit: TimeUnit.Year, culture: CultureInfo.GetCultureInfo("id-Id"));
    }

    #region Cron

    /// <summary>
    /// Returns cron expression that fires every &lt;<paramref name="interval"></paramref>&gt; minutes.
    /// </summary>
    /// <param name="interval">The number of minutes to wait between every activation.</param>
    public static string MinuteInterval(int interval) => $"*/{interval} * * * *";

    /// <summary>
    /// Returns cron expression that fires every &lt;<paramref name="interval"></paramref>&gt; hours.
    /// </summary>
    /// <param name="interval">The number of hours to wait between every activation.</param>
    public static string HourInterval(int interval) => $"0 */{interval} * * *";

    /// <summary>
    /// Returns cron expression that fires every &lt;<paramref name="interval"></paramref>&gt; days.
    /// </summary>
    /// <param name="interval">The number of days to wait between every activation.</param>
    public static string DayInterval(int interval) => $"0 0 */{interval} * *";

    /// <summary>
    /// Returns cron expression that fires every &lt;<paramref name="interval"></paramref>&gt; months.
    /// </summary>
    /// <param name="interval">The number of months to wait between every activation.</param>
    public static string MonthInterval(int interval) => $"0 0 1 */{interval} *";

    #endregion

}