using System.ComponentModel;
using System.Reflection;
using ZiziBot.Contracts.Dtos;

// ReSharper disable InconsistentNaming
namespace ZiziBot.Contracts.Constants;

public static class Flag
{
    [DefaultValue(true)]
    public const string CONSOLE = "CONSOLE";

    [DefaultValue(true)]
    public const string HANGFIRE = "HANGFIRE";

    [DefaultValue(true)]
    public const string RSS_BROADCASTER = "RSS_BROADCASTER";

    [DefaultValue(false)]
    public const string SALAT_TIME = "SALAT_TIME";

    [DefaultValue(false)]
    public const string ANTI_RAID = "ANTI_RAID";

    [DefaultValue(false)]
    public const string RSS_INCLUDE_CONTENT = "RSS_INCLUDE_CONTENT";

    [DefaultValue(true)]
    public const string RSS_RESET_AT_STARTUP = "RSS_RESET_AT_STARTUP";

    [DefaultValue(true)]
    public const string COMMAND_PING = "COMMAND_PING";

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