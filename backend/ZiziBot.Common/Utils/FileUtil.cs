using System.Collections.Concurrent;
using System.Text;

namespace ZiziBot.Common.Utils;

public static class FileUtil
{
    private static readonly ConcurrentDictionary<string, ReaderWriterLockSlim> FileLocks = new ConcurrentDictionary<string, ReaderWriterLockSlim>();
    private static readonly TimeSpan LockTimeout = TimeSpan.FromSeconds(60);
    private static readonly Lock LockCleanupLock = new Lock();
    private static DateTime _lastCleanup = DateTime.UtcNow;

    private static ReaderWriterLockSlim GetFileLock(string filePath)
    {
        CleanupOldLocks();
        // Use LockRecursionPolicy.SupportsRecursion to allow recursive lock acquires on the same thread
        return FileLocks.GetOrAdd(filePath, _ => new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion));
    }

    private static void CleanupOldLocks()
    {
        // Only cleanup every minute to avoid excessive locking
        if (DateTime.UtcNow - _lastCleanup < TimeSpan.FromMinutes(1))
            return;

        lock (LockCleanupLock)
        {
            if (DateTime.UtcNow - _lastCleanup < TimeSpan.FromMinutes(1))
                return;

            var keysToRemove = FileLocks
                .Where(pair => !pair.Value.IsReadLockHeld && !pair.Value.IsWriteLockHeld && !pair.Value.IsUpgradeableReadLockHeld)
                .Select(pair => pair.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                if (FileLocks.TryRemove(key, out var lockToDispose))
                {
                    try
                    {
                        lockToDispose.Dispose();
                    }
                    catch
                    {
                        /* Ignore disposal errors */
                    }
                }
            }

            _lastCleanup = DateTime.UtcNow;
        }
    }

    public static async Task WriteAllTextThreadSafeAsync(this string path, string contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        encoding ??= Encoding.UTF8;

        var fileLock = GetFileLock(path);
        bool upgradeableLockAcquired = false;
        bool writeLockAcquired = false;

        try
        {
            // First, acquire an upgradeable read lock
            upgradeableLockAcquired = fileLock.TryEnterUpgradeableReadLock(LockTimeout);
            if (!upgradeableLockAcquired)
                throw new TimeoutException($"Failed to acquire upgradeable read lock for file: {path}");

            // Then upgrade to a write lock if needed
            writeLockAcquired = fileLock.TryEnterWriteLock(LockTimeout);
            if (!writeLockAcquired)
                throw new TimeoutException($"Failed to acquire write lock for file: {path}");

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
            // Release write lock if acquired
            if (writeLockAcquired && fileLock.IsWriteLockHeld)
                fileLock.ExitWriteLock();

            // Release upgradeable read lock if acquired
            if (upgradeableLockAcquired && fileLock.IsUpgradeableReadLockHeld)
                fileLock.ExitUpgradeableReadLock();
        }
    }

    public static async Task<string> ReadAllTextThreadSafeAsync(this string path, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        encoding ??= Encoding.UTF8;

        var fileLock = GetFileLock(path);
        bool lockAcquired = false;

        try
        {
            lockAcquired = fileLock.TryEnterReadLock(LockTimeout);
            if (!lockAcquired)
                throw new TimeoutException($"Failed to acquire read lock for file: {path}");

            return await File.ReadAllTextAsync(path, encoding, cancellationToken);
        }
        catch (Exception) when (lockAcquired)
        {
            // If we get an exception after acquiring the lock, release it before rethrowing
            fileLock.ExitReadLock();
            throw;
        }
        finally
        {
            if (lockAcquired && fileLock.IsReadLockHeld)
                fileLock.ExitReadLock();
        }
    }

    public static void WriteAllTextThreadSafe(this string path, string contents, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        var fileLock = GetFileLock(path);
        bool upgradeableLockAcquired = false;
        bool writeLockAcquired = false;

        try
        {
            // First, acquire an upgradeable read lock
            upgradeableLockAcquired = fileLock.TryEnterUpgradeableReadLock(LockTimeout);
            if (!upgradeableLockAcquired)
                throw new TimeoutException($"Failed to acquire upgradeable read lock for file: {path}");

            // Then upgrade to a write lock if needed
            writeLockAcquired = fileLock.TryEnterWriteLock(LockTimeout);
            if (!writeLockAcquired)
                throw new TimeoutException($"Failed to acquire write lock for file: {path}");

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
            // Release write lock if acquired
            if (writeLockAcquired && fileLock.IsWriteLockHeld)
                fileLock.ExitWriteLock();

            // Release upgradeable read lock if acquired
            if (upgradeableLockAcquired && fileLock.IsUpgradeableReadLockHeld)
                fileLock.ExitUpgradeableReadLock();
        }
    }

    public static string ReadAllTextThreadSafe(this string path, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        var fileLock = GetFileLock(path);
        bool lockAcquired = false;

        try
        {
            lockAcquired = fileLock.TryEnterReadLock(LockTimeout);
            if (!lockAcquired)
                throw new TimeoutException($"Failed to acquire read lock for file: {path}");

            return File.ReadAllText(path, encoding);
        }
        catch (Exception) when (lockAcquired)
        {
            // If we get an exception after acquiring the lock, release it before rethrowing
            fileLock.ExitReadLock();
            throw;
        }
        finally
        {
            if (lockAcquired && fileLock.IsReadLockHeld)
                fileLock.ExitReadLock();
        }
    }

    public static void DeleteThreadSafe(this string path, bool throwOnError = false)
    {
        var fileLock = GetFileLock(path);
        bool upgradeableLockAcquired = false;
        bool writeLockAcquired = false;

        try
        {
            if (!File.Exists(path))
                return;

            // First, acquire an upgradeable read lock
            upgradeableLockAcquired = fileLock.TryEnterUpgradeableReadLock(LockTimeout);

            if (!upgradeableLockAcquired)
            {
                if (throwOnError)
                    throw new TimeoutException($"Failed to acquire upgradeable read lock for file: {path}");

                return;
            }

            // Then upgrade to a write lock
            writeLockAcquired = fileLock.TryEnterWriteLock(LockTimeout);

            if (!writeLockAcquired)
            {
                if (throwOnError)
                    throw new TimeoutException($"Failed to acquire write lock for file: {path}");

                return;
            }

            File.Delete(path);
        }
        catch when (!throwOnError)
        {
            // Ignore errors if throwOnError is false
        }
        finally
        {
            // Release write lock if acquired
            if (writeLockAcquired && fileLock.IsWriteLockHeld)
                fileLock.ExitWriteLock();

            // Release upgradeable read lock if acquired
            if (upgradeableLockAcquired && fileLock.IsUpgradeableReadLockHeld)
                fileLock.ExitUpgradeableReadLock();
        }
    }

    public static async Task DeleteThreadSafeAsync(this string path, bool throwOnError = false, CancellationToken cancellationToken = default)
    {
        var fileLock = GetFileLock(path);
        bool upgradeableLockAcquired = false;
        bool writeLockAcquired = false;

        try
        {
            if (!File.Exists(path))
                return;

            // First, acquire an upgradeable read lock
            upgradeableLockAcquired = fileLock.TryEnterUpgradeableReadLock(LockTimeout);

            if (!upgradeableLockAcquired)
            {
                if (throwOnError)
                    throw new TimeoutException($"Failed to acquire upgradeable read lock for file: {path}");

                return;
            }

            // Then upgrade to a write lock
            writeLockAcquired = fileLock.TryEnterWriteLock(LockTimeout);

            if (!writeLockAcquired)
            {
                if (throwOnError)
                    throw new TimeoutException($"Failed to acquire write lock for file: {path}");

                return;
            }

            await Task.Run(() => File.Delete(path), cancellationToken);
        }
        catch when (!throwOnError)
        {
            // Ignore errors if throwOnError is false
        }
        finally
        {
            // Release write lock if acquired
            if (writeLockAcquired && fileLock.IsWriteLockHeld)
                fileLock.ExitWriteLock();

            // Release upgradeable read lock if acquired
            if (upgradeableLockAcquired && fileLock.IsUpgradeableReadLockHeld)
                fileLock.ExitUpgradeableReadLock();
        }
    }
}