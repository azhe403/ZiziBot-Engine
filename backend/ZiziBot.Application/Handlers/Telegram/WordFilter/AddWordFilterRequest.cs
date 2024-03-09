using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.WordFilter;

public class AddWordFilterRequest : BotRequestBase
{
    public string? Word { get; set; }
}

public class AddWordFilterHandler : IBotRequestHandler<AddWordFilterRequest>
{
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly TelegramService _telegramService;

    public AddWordFilterHandler(MongoDbContextBase mongoDbContext, TelegramService telegramService)
    {
        _mongoDbContext = mongoDbContext;
        _telegramService = telegramService;
    }

    public async Task<BotResponseBase> Handle(AddWordFilterRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        if (request.Word.IsNullOrEmpty())
        {
            return await _telegramService.SendMessageAsync("Apa kata yang ingin ditambahkan?");
        }

        var wordFilter = await _mongoDbContext.WordFilter
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .Where(entity => entity.Word == request.Word)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (wordFilter == null)
        {
            _mongoDbContext.WordFilter.Add(new WordFilterEntity()
            {
                Word = request.Word,
                Status = (int)EventStatus.Complete,
                TransactionId = request.TransactionId
            });

            await _mongoDbContext.SaveChangesAsync(cancellationToken);

            return await _telegramService.SendMessageAsync("Kata berhasil disimpan");
        }

        return await _telegramService.SendMessageAsync("Kata sudah disimpan");
    }
}