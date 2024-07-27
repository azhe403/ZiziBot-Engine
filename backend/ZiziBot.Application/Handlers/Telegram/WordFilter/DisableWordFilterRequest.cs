using MongoFramework.Linq;

public class DisableWordFilterRequest : BotRequestBase
{
    public string? Word { get; set; }
}

public class DisableWordFilterHandler(
    TelegramService telegramService,
    MongoDbContextBase mongoDbContext,
    WordFilterRepository wordFilterRepository
) : IBotRequestHandler<DisableWordFilterRequest>
{
    public async Task<BotResponseBase> Handle(DisableWordFilterRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        if (request.Word == null)
        {
            return await telegramService.SendMessageAsync("Apa Word yang ingin dimatikan?");
        }

        var wordFilter = await mongoDbContext.WordFilter
            .Where(x => x.Word == request.Word)
            .Where(x => x.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (wordFilter == null)
        {
            return await telegramService.SendMessageAsync("Word tidak ditemukan");
        }

        wordFilter.Status = (int)EventStatus.Inactive;

        await mongoDbContext.SaveChangesAsync(cancellationToken);
        await wordFilterRepository.GetAllAsync(true);

        return await telegramService.SendMessageAsync("Word berhasil dimatikan");
    }
}