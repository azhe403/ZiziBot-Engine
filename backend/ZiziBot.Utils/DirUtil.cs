using System.IO.Compression;

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

    public static string GetCurrentDirectory(this string path)
    {
        var dirName = new DirectoryInfo(path).Name;
        return dirName;
    }

    public static string GetFileName(this string path)
    {
        return Path.GetFileName(path);
    }

    public static string CompressToZip(this string dirSource, string? fileName = null, bool deletePrev = true)
    {
        var zipFileName = dirSource.TrimEnd('/') + ".zip";
        if (fileName != null)
        {
            var dirName = dirSource.GetCurrentDirectory();
            zipFileName = Path.Combine(dirSource.Replace(dirName, ""), fileName);
        }

        if (deletePrev)
            File.Delete(zipFileName);

        ZipFile.CreateFromDirectory(dirSource, zipFileName);

        return zipFileName;
    }
}