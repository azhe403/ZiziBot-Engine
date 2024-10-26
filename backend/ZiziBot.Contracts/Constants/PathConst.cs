using System.Diagnostics.CodeAnalysis;

namespace ZiziBot.Contracts.Constants;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class PathConst
{
    public const string TEMP_PATH = "Storage/Temp/";

    public const string CACHE_TOWER_JSON = "Storage/CacheTower/Json/";
    public const string CACHE_TOWER_PATH = "Storage/CacheTower/File/";
    public const string CACHE_TOWER_SQLITE_PATH = "Storage/CacheTower/Sqlite.db";

    public const string HANGFIRE_SQLITE_PATH = "Storage/Hangfire/Sqlite.db";
    public const string HANGFIRE_LITEDB_PATH = "Storage/Hangfire/Lite.db";

    public readonly static string BACKUP = Path.Combine(Environment.CurrentDirectory, "Storage", "Backup");
    public readonly static string MONGODB_BACKUP = Path.Combine(Environment.CurrentDirectory, "Storage", "Backup", "MongoDB/");
}