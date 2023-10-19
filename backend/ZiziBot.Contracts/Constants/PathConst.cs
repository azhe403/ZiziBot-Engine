using System.Diagnostics.CodeAnalysis;

namespace ZiziBot.Contracts.Constants;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class PathConst
{
    public const string TEMP_PATH = "Storage/Temp/";

    public static readonly string CACHE_TOWER_PATH = "Storage/CacheTower/File/";
    public static readonly string CACHE_TOWER_SQLITE_PATH = "Storage/CacheTower/Sqlite.db";

    public static readonly string MONGODB_BACKUP = Path.Combine(Environment.CurrentDirectory, "Storage", "Backup", "MongoDB/");
}