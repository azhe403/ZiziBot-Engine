using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.DataSource.MongoEf;

public class MongoEfContext : DbContext
{
    private readonly string _connectionString = EnvUtil.GetEnv(Env.MONGODB_CONNECTION_STRING, throwIsMissing: true);

    public DbSet<WebhookHistoryEntity> WebhookHistory { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conn = MongoUrl.Create(_connectionString);

        optionsBuilder.UseMongoDB(_connectionString, conn.DatabaseName);
    }
}