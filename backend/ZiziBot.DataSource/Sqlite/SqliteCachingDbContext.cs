using Microsoft.EntityFrameworkCore;
using ZiziBot.DataSource.Sqlite.Entities;

namespace ZiziBot.DataSource.Sqlite;

public class SqliteCachingDbContext : DbContext
{
    private readonly string _dbPath;

    public DbSet<SqliteCacheEntity> SqliteCache { get; set; }

    public SqliteCachingDbContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        optionsBuilder.EnableSensitiveDataLogging();
    }
}