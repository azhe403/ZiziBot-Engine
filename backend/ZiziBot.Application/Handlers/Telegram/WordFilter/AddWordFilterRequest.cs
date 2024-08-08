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

        List<WordFilterAction> action = new();

        var cmdParam = request.Params?.Skip(1).ToList();
        if (cmdParam.NotEmpty())
        {
            if (cmdParam.Contains("-d"))
                action.Add(WordFilterAction.Delete);

            if (cmdParam.Contains("-w"))
                action.Add(WordFilterAction.Warn);

            if (cmdParam.Contains("-m"))
                action.Add(WordFilterAction.Mute);

            if (cmdParam.Contains("-k"))
                action.Add(WordFilterAction.Kick);
        }

        await wordFilterRepository.SaveAsync(new WordFilterDto() {
            ChatId = request.ChatIdentifier,
            UserId = request.UserId,
            Word = request.Word,
            Action = action.ToArray(),
            TransactionId = request.TransactionId
        });

        return await telegramService.SendMessageAsync("Kata berhasil disimpan");
    }
}