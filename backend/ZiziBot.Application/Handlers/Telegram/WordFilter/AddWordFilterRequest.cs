using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.WordFilter;

public class AddWordFilterRequest : BotRequestBase
{
    public string? Word { get; set; }
}

public class AddWordFilterHandler(
    TelegramService telegramService,
    WordFilterRepository wordFilterRepository
) : IBotRequestHandler<AddWordFilterRequest>
{
    public async Task<BotResponseBase> Handle(AddWordFilterRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        if (request.Word.IsNullOrEmpty())
        {
            return await telegramService.SendMessageAsync("Apa kata yang ingin ditambahkan?");
        }

        await wordFilterRepository.Save(new WordFilterEntity() {
            Word = request.Word,
            Status = (int)EventStatus.Complete,
            TransactionId = request.TransactionId
        });

        return await telegramService.SendMessageAsync("Kata berhasil disimpan");
    }
}