using System.Text;

namespace ZiziBot.Common.Utils
{
    public static class FileUtil
    {
        private static readonly ReaderWriterLockSlim Lock = new();

        public static async Task WriteAllTextThreadSafeAsync(this string path, string contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
        {
            encoding ??= Encoding.UTF8;

            try
            {
                Lock.EnterWriteLock();

                // Ensure directory exists
                var directory = Path.GetDirectoryName(path);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write to a temporary file first
                var tempFile = Path.GetTempFileName();

                try
                {
                    await File.WriteAllTextAsync(tempFile, contents, encoding, cancellationToken);

                    // Then replace the original file
                    File.Move(tempFile, path, overwrite: true);
                }
                finally
                {
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch
                    {
                        /* Ignore cleanup errors */
                    }
                }
            }
            finally
            {
                if (Lock.IsWriteLockHeld)
                    Lock.ExitWriteLock();
            }
        }

        public static async Task<string> ReadAllTextThreadSafeAsync(this string path, Encoding? encoding = null, CancellationToken cancellationToken = default)
        {
            encoding ??= Encoding.UTF8;

            try
            {
                Lock.EnterReadLock();
                return await File.ReadAllTextAsync(path, encoding, cancellationToken);
            }
            finally
            {
                if (Lock.IsReadLockHeld)
                    Lock.ExitReadLock();
            }
        }

        public static void WriteAllTextThreadSafe(this string path, string contents, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;

            try
            {
                Lock.EnterWriteLock();

                // Ensure directory exists
                var directory = Path.GetDirectoryName(path);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write to a temporary file first
                var tempFile = Path.GetTempFileName();

                try
                {
                    File.WriteAllText(tempFile, contents, encoding);

                    // Then replace the original file
                    File.Move(tempFile, path, overwrite: true);
                }
                finally
                {
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch
                    {
                        /* Ignore cleanup errors */
                    }
                }
            }
            finally
            {
                if (Lock.IsWriteLockHeld)
                    Lock.ExitWriteLock();
            }
        }

        public static string ReadAllTextThreadSafe(this string path, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;

            try
            {
                Lock.EnterReadLock();
                return File.ReadAllText(path, encoding);
            }
            finally
            {
                if (Lock.IsReadLockHeld)
                    Lock.ExitReadLock();
            }
        }

        public static void DeleteThreadSafe(this string path, bool throwOnError = false)
        {
            try
            {
                if (!File.Exists(path))
                    return;

                Lock.EnterWriteLock();
                File.Delete(path);
            }
            catch when (!throwOnError)
            {
                // Ignore errors if throwOnError is false
            }
            finally
            {
                if (Lock.IsWriteLockHeld)
                    Lock.ExitWriteLock();
            }
        }

        public static async Task DeleteThreadSafeAsync(this string path, bool throwOnError = false, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!File.Exists(path))
                    return;

                await Task.Run(() => {
                    try
                    {
                        Lock.EnterWriteLock();
                        File.Delete(path);
                    }
                    finally
                    {
                        if (Lock.IsWriteLockHeld)
                            Lock.ExitWriteLock();
                    }
                }, cancellationToken);
            }
            catch when (!throwOnError)
            {
                // Ignore errors if throwOnError is false
            }
        }
    }
}