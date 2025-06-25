using System.IO.Compression;
using Serilog;

namespace ZiziBot.Common.Utils;

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

    public static string DeleteDirectory(this string path)
    {
        try
        {
            Directory.Delete(path, true);
        }
        catch (Exception e)
        {
            Log.Debug(e, "Failed to delete directory: {Path} with error: {Message}", path, e.Message);
        }

        return path;
    }

    public static string DeleteFile(this string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception e)
        {
            Log.Debug(e, "Failed to delete directory: {Path} with error: {Message}", path, e.Message);
        }

        return path;
    }

    public static int DeleteFile(this List<FileInfo> listFile)
    {
        foreach (var fileInfo in listFile)
        {
            Log.Debug("Delete file: {FullName}", fileInfo.FullName);

            fileInfo.Delete();
        }

        return listFile.Count;
    }

    public static string GetCurrentDirectory(this string path)
    {
        var dirName = new DirectoryInfo(path).Name;
        return dirName;
    }

    public static string GetFileName(this string? path)
    {
        if (path.IsNullOrWhiteSpace())
            return string.Empty;

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

    public static List<FileInfo> GetFiles(this string dirPath, string pattern = "*.*", Func<FileInfo, bool>? predicate = null)
    {
        return Directory.EnumerateFiles(dirPath, pattern, SearchOption.AllDirectories).Select(x => new FileInfo(x))
            .WhereIf(predicate != null, predicate)
            .ToList();
    }
}