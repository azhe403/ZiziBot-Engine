using System.ComponentModel;
using System.Reflection;
using Serilog;
using ZiziBot.Contracts.Dtos;

// ReSharper disable InconsistentNaming
namespace ZiziBot.Contracts.Constants;

public static class Flag
{
    public static List<FlagDto>? Current { get; set; }

    public static List<FlagDto> GetFields()
    {
        var properties = typeof(Flag).GetFields();

        var values = properties.Select(x => new FlagDto() {
            Name = x.Name,
            Value = (bool)(x.GetCustomAttribute<DefaultValueAttribute>()?.Value ?? false)
        }).ToList();

        return values;
    }

    public static bool IsEnabled(string flagName)
    {
        var flag = Current?.FirstOrDefault(x => x.Name == flagName);

        if (flag == null)
        {
            var defaultFlag = GetFields().FirstOrDefault(x => x.Name == flagName);
            var flagValue = defaultFlag?.Value ?? false;
            Log.Debug("Flag {FlagName} not found. Using default value: {DefaultFlagValue}", flagName, flagValue);

            return flagValue;
        }

        Log.Debug("Flag {FlagName} is {FlagValue}", flag.Name, flag.Value);
        return flag.Value;
    }

    #region Flags
    #region Infrastructure
    [DefaultValue(true)]
    public const string CONSOLE_BLAZOR = "CONSOLE_BLAZOR";

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
    #endregion

    #region Bot Commands
    [DefaultValue(true)]
    public const string COMMAND_PING = "COMMAND_PING";

    [DefaultValue(true)]
    public const string COMMAND_SP = "COMMAND_SP";

    [DefaultValue(true)]
    public const string COMMAND_MP = "COMMAND_MP";
    #endregion

    #region Rest API
    [DefaultValue(true)]
    public const string REST_USER_INFO_GET = "REST_USER_INFO_GET";

    [DefaultValue(true)]
    public const string REST_USER_TELEGRAM_SESSION_CREATE = "REST_USER_TELEGRAM_SESSION_CREATE";

    [DefaultValue(true)]
    public const string REST_USER_SESSION_OTP_POST = "REST_USER_SESSION_OTP_POST";

    [DefaultValue(true)]
    public const string REST_USER_GROUP_LIST = "REST_USER_GROUP_LIST";

    [DefaultValue(true)]
    public const string REST_CHAT_NOTE_CREATE = "REST_CHAT_NOTE_CREATE";

    [DefaultValue(true)]
    public const string REST_CHAT_NOTE_LIST = "REST_CHAT_NOTE_LIST";

    [DefaultValue(true)]
    public const string REST_CHAT_NOTE_GET = "REST_CHAT_NOTE_GET";

    [DefaultValue(true)]
    public const string REST_CHAT_RSS_LIST = "REST_CHAT_RSS_LIST";

    [DefaultValue(true)]
    public const string REST_CHAT_RSS_CREATE = "REST_CHAT_RSS_CREATE";

    [DefaultValue(true)]
    public const string REST_CHAT_NOTE_DELETE = "REST_CHAT_NOTE_DELETE";

    [DefaultValue(true)]
    public const string REST_ANTISPAM_ESS_CREATE = "REST_ANTISPAM_ESS_CREATE";

    [DefaultValue(true)]
    public const string REST_ANTISPAM_ESS_DELETE = "REST_ANTISPAM_ESS_DELETE";

    [DefaultValue(true)]
    public const string REST_GROUP_WELCOME_MESSAGE_CREATE = "REST_GROUP_WELCOME_MESSAGE_CREATE";

    [DefaultValue(true)]
    public const string REST_GROUP_WELCOME_MESSAGE_UPDATE = "REST_GROUP_WELCOME_MESSAGE_UPDATE";

    [DefaultValue(true)]
    public const string REST_GROUP_WELCOME_MESSAGE_SELECT = "REST_GROUP_WELCOME_MESSAGE_SELECT";

    [DefaultValue(true)]
    public const string REST_GROUP_WELCOME_MESSAGE_GET_LIST = "REST_GROUP_WELCOME_MESSAGE_GET_LIST";

    [DefaultValue(true)]
    public const string REST_GROUP_WELCOME_MESSAGE_GET_DETAIL = "REST_GROUP_WELCOME_MESSAGE_GET_DETAIL";

    [DefaultValue(true)]
    public const string REST_GROUP_WELCOME_MESSAGE_DELETE = "REST_GROUP_WELCOME_MESSAGE_DELETE";

    [DefaultValue(true)]
    public const string REST_MIRROR_USER_CREATE = "REST_MIRROR_USER_CREATE";

    [DefaultValue(true)]
    public const string REST_MIRROR_SUBMIT_DONATION = "REST_MIRROR_SUBMIT_DONATION";

    [DefaultValue(true)]
    public const string REST_MIRROR_USER_CHECK_USER = "REST_MIRROR_USER_CHECK_USER";

    [DefaultValue(true)]
    public const string REST_MIRROR_USER_CHECK_ORDER = "REST_MIRROR_USER_CHECK_ORDER";

    [DefaultValue(true)]
    public const string REST_MIRROR_USER_GET_LIST = "REST_MIRROR_USER_GET_LIST";

    [DefaultValue(true)]
    public const string REST_MIRROR_USER_DELETE = "REST_MIRROR_USER_DELETE";

    [DefaultValue(true)]
    public const string REST_PRODUCTIVITY_PENDEKIN_CREATE = "REST_PRODUCTIVITY_PENDEKIN_CREATE";

    [DefaultValue(true)]
    public const string REST_PRODUCTIVITY_PENDEKIN_LIST = "REST_PRODUCTIVITY_PENDEKIN_LIST";

    [DefaultValue(true)]
    public const string REST_PRODUCTIVITY_PENDEKIN_GET = "REST_PRODUCTIVITY_PENDEKIN_GET";

    [DefaultValue(true)]
    public const string REST_PRODUCTIVITY_WEBHOOK = "REST_PRODUCTIVITY_WEBHOOK";
    #endregion
    #endregion
}