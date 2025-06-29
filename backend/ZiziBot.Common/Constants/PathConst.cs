namespace ZiziBot.Common.Constants;

public static class PathConst
{
    public const string TEMP_PATH = "Storage/Temp/";

    public const string CACHE_TOWER_JSON = "Storage/CacheTower/Json/";
    public const string CACHE_TOWER_PATH = "Storage/CacheTower/File/";
    public const string CACHE_TOWER_SQLITE_PATH = "Storage/CacheTower/Sqlite/Sqlite.db";

    public const string HANGFIRE_SQLITE_PATH = "Storage/Hangfire/Sqlite/Sqlite.db";
    public const string HANGFIRE_LITEDB_PATH = "Storage/Hangfire/LiteDb/LiteDb.db";

    public static readonly string BACKUP = Path.Combine(Environment.CurrentDirectory, "Storage", "Backup");
    public static readonly string MONGODB_BACKUP = Path.Combine(BACKUP, "MongoDB/");
}