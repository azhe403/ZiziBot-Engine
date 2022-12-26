using System.Diagnostics.CodeAnalysis;

namespace ZiziBot.Contracts.Constants;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class Env
{
    public const string AZURE_APP_CONFIG_CONNECTION_STRING = "AZURE_APP_CONFIG_CONNECTION_STRING";
    public const string MONGODB_CONNECTION_STRING = "MONGODB_CONNECTION_STRING";
    public const string TELEGRAM_WEBHOOK_URL = "TELEGRAM_WEBHOOK_URL";

    public const string DASHBOARD_PROJECT_PATH = "../ZiziBot.NgDashboard/ng-dashboard";
}