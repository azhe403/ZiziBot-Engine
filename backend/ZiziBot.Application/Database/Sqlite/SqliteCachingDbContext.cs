using Microsoft.EntityFrameworkCore;
using ZiziBot.Application.Database.Sqlite.Entities;

namespace ZiziBot.Application.Database.Sqlite;

public class SqliteCachingDbContext(string dbPath) : DbContext
{
    public DbSet<SqliteCacheEntity> SqliteCache { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        optionsBuilder.EnableSensitiveDataLogging();
    }
}
