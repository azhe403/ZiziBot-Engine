using System.Diagnostics.CodeAnalysis;

namespace ZiziBot.Contracts.Constants;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class UrlConst
{
    public static readonly string WEBHOOK_URL = EnvUtil.GetEnv(Env.WEB_CONSOLE_URL) + "/api/webhook/";

    public const string OCR_URL_API = "https://api.optiic.dev/process";

    public const string ANTISPAM_COMBOT_API = "https://api.cas.chat/check";
    public const string ANTISPAM_SPAMWATCH_API = "https://api.spamwat.ch/banlist";

    public const string BOT_API_SPEC = "https://raw.githubusercontent.com/PaulSonOfLars/telegram-bot-api-spec/main/api.min.json";

    public const string API_TRAKTEER_PARSER = "https://php-code.azhe.my.id/trakteer.php";

    public const string DOC_MIRROR_VERIFY_DONATION = "https://docs.mirror.winten.my.id/verifikasi-donasi";

    public const string API_BINDERBYTE = "https://api.binderbyte.com/v1/";
    public const string API_TONJOO_ONGKIR = "https://pluginongkoskirim.com";
}