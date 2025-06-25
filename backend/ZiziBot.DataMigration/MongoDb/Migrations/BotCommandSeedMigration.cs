using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Telegram.Bot.Types.Enums;
using ZiziBot.Common.Enums;
using ZiziBot.DataMigration.MongoDb.Interfaces;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.DataMigration.MongoDb.Migrations;

public class BotCommandSeedMigration(ILogger<BotCommandSeedMigration> logger, MongoEfContext mongoDbContext) : IPostMigration
{
    readonly ILogger<BotCommandSeedMigration> _logger = logger;

    public async Task DownAsync(IMongoDatabase db)
    {
        await Task.Delay(1);
    }

    public async Task UpAsync(IMongoDatabase db)
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

        await mongoDbContext.SaveChangesAsync();
    }
}