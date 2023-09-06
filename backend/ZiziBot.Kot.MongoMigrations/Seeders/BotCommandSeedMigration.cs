using Kot.MongoDB.Migrations;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Kot.MongoMigrations.Seeders;

public class BotCommandSeedMigration : MongoMigration
{
    private readonly ILogger<BotCommandSeedMigration> _logger;
    private readonly MongoDbContextBase _mongoDbContext;
    private static DatabaseVersion DBVersion => new("233.30.1");

    public BotCommandSeedMigration(ILogger<BotCommandSeedMigration> logger, MongoDbContextBase mongoDbContext) : base(DBVersion)
    {
        _logger = logger;
        _mongoDbContext = mongoDbContext;
    }

    public override async Task DownAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
    }

    public override async Task UpAsync(IMongoDatabase db, IClientSessionHandle session, CancellationToken cancellationToken)
    {
        _mongoDbContext.BotCommand.AddRange(new[]
        {
            new BotCommandEntity()
            {
                Command = "/ping",
                Description = "Mengecek kesehatan bot",
                Scope = BotCommandScopeType.Default,
                Status = (int)EventStatus.Complete
            },
            new BotCommandEntity()
            {
                Command = "/help",
                Description = "Menampilkan daftar perintah",
                Scope = BotCommandScopeType.Default,
                Status = (int)EventStatus.Complete
            },
            new BotCommandEntity()
            {
                Command = "/start",
                Description = "Memulai menggunakan bot",
                Scope = BotCommandScopeType.Default,
                Status = (int)EventStatus.Complete
            },
        });

        await _mongoDbContext.SaveChangesAsync(cancellationToken);
    }
}