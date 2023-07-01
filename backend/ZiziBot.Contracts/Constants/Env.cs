using System.Diagnostics.CodeAnalysis;

namespace ZiziBot.Contracts.Constants;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class Env
{
    public const string AZURE_APP_CONFIG_CONNECTION_STRING = "AZURE_APP_CONFIG_CONNECTION_STRING";
    public const string MONGODB_CONNECTION_STRING = "MONGODB_CONNECTION_STRING";
    public const string TELEGRAM_WEBHOOK_URL = "TELEGRAM_WEBHOOK_URL";

    public const string WEB_CONSOLE_URL = "WEB_CONSOLE_URL";
    public static readonly string WEB_VERIFY_SESSION_URL = EnvUtil.GetEnv(WEB_CONSOLE_URL);

    public const string DASHBOARD_PROJECT_PATH = "./../ng-dashboard";
    public const string DASHBOARD_DIST_PATH = $"{DASHBOARD_PROJECT_PATH}/dist/ng-dashboard";

    public const string COMMON_UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

    public const int DEFAULT_TIMEZONE = 7;
}