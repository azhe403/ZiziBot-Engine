using System.ComponentModel;
using System.Reflection;
using ZiziBot.Contracts.Dtos;

// ReSharper disable InconsistentNaming

namespace ZiziBot.Contracts.Constants;

public class Flag
{
    [DefaultValue(true)] public static string CONSOLE = "CONSOLE";

    [DefaultValue(true)] public static string HANGFIRE = "HANGFIRE";

    [DefaultValue(true)] public static string RSS_BROADCASTER = "RSS_BROADCASTER";

    [DefaultValue(false)] public static string SALAT_TIME = "SALAT_TIME";

    [DefaultValue(false)] public static string ANTI_RAID = "ANTI_RAID";

    [DefaultValue(false)] public static string RSS_INCLUDE_CONTENT = "RSS_INCLUDE_CONTENT";

    [DefaultValue(true)] public static string RSS_RESET_AT_STARTUP = "RSS_RESET_AT_STARTUP";

    public static List<FlagDto> GetFields()
    {
        var properties = typeof(Flag).GetFields();

        var values = properties.Select(x => new FlagDto() {
            Name = x.Name,
            Value = (bool)(x.GetCustomAttribute<DefaultValueAttribute>()?.Value ?? false)
        }).ToList();

        return values;
    }
}