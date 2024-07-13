using System.Diagnostics.CodeAnalysis;

namespace ZiziBot.Contracts.Constants;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class CacheKey
{
    public const string GLOBAL_SUDO = "global/sudo/";
    public const string GLOBAL_API_DOC = "global/api-doc";

    public const string USER_BAN_ESS = "user/ban/ess/";
    public const string USER_BAN_CAS = "user/ban/cas/";
    public const string USER_BAN_SW = "user/ban/sw/";
    public const string USER_ACTIVE_USERNAMES = "user/active-usernames/";

    public const string CHAT_ADMIN = "chat/admin/";
    public const string CHAT_NOTES = "chat/notes/";
    public const string CHAT_ACTIVE_USERNAMES = "chat/active-usernames/";
    public const string CHAT_RESTRICTION = "chat/restriction/";
    public const string CHAT_BADWORD = "chat/badword";
}