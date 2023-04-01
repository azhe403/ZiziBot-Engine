using System.Reflection;

namespace ZiziBot.Parsers;

public static class VersionUtil
{
    public static int GetBuildNumber()
    {
        var jan2000 = new DateTime(2000, 1, 1);
        var today = DateTime.UtcNow;
        var diffDay = (today - jan2000).Days;

        return diffDay;
    }

    public static double GetRevNumber()
    {
        var dateNow = DateTime.UtcNow.ToString("h:mm:ss");
        var seconds = TimeSpan.Parse(dateNow).TotalSeconds;
        return seconds;
    }

    public static string GetVersion(bool pretty = false)
    {
        var currentAssembly = Assembly.GetCallingAssembly().GetName();
        var version = currentAssembly.Version;

        return pretty ? $"{version.Major}.{version.Minor} Build {version.Build}" : version.ToString();

    }

    public static DateTime GetBuildDate(this Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<BuildDateAttribute>();
        return attribute != null ? attribute.BuildDate : default(DateTime);
    }

    public static DateTime GetLinkerTime(this Assembly assembly)
    {
        return File.GetLastWriteTime(assembly.Location);
    }

    public static DateTime GetBuildDate()
    {
        var buildDate = Assembly.GetEntryAssembly().GetBuildDate();
        return buildDate;
    }
}