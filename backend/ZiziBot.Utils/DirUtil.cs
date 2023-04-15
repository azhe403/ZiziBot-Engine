namespace ZiziBot.Utils;

public static class DirUtil
{
    public static string EnsureDirectory(this string path)
    {
        var directory = Path.GetDirectoryName(path);

        if (Directory.Exists(directory)) return path;
        if (string.IsNullOrEmpty(directory)) return path;

        Directory.CreateDirectory(directory);

        return path;
    }
}