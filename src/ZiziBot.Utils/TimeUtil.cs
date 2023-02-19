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
}