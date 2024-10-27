using Kot.MongoDB.Migrations;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Telegram.Bot.Types.Enums;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Kot.MongoMigrations.Seeders;

public class BotCommandSeedMigration(ILogger<BotCommandSeedMigration> logger, MongoEfContext mongoDbContext) : MongoMigration(DBVersion)
{
    readonly ILogger<BotCommandSeedMigration> _logger = logger;
    static DatabaseVersion DBVersion => new("233.30.1");

    public override async Task DownAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
    }

    public override async Task UpAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken)
    {
        mongoDbContext.BotCommand.AddRange(new List<BotCommandEntity>() {
            new() {
                Command = "/ping",
                Description = "Mengecek kesehatan bot",
                Scope = BotCommandScopeType.Default,
                Status = EventStatus.Complete
            },
            new() {
                Command = "/help",
                Description = "Menampilkan daftar perintah",
                Scope = BotCommandScopeType.Default,
                Status = EventStatus.Complete
            },
            new() {
                Command = "/start",
                Description = "Memulai menggunakan bot",
                Scope = BotCommandScopeType.Default,
                Status = EventStatus.Complete
            }
        });

        await mongoDbContext.SaveChangesAsync(cancellationToken);
    }
}