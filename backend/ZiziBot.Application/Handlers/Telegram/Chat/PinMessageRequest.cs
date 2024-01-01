namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class PinMessageRequest : BotRequestBase
{
}

public class PinMessageHandler : IBotRequestHandler<PinMessageRequest>
{
    private readonly TelegramService _telegramService;

    public PinMessageHandler(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<BotResponseBase> Handle(PinMessageRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        if (request.ReplyToMessage == null)
        {
            return await _telegramService.SendMessageAsync("Spesifikan pesan yang ingin dipin..");
        }

        await _telegramService.SendMessageAsync("📍 Sedang mengepin pesan..");

        await _telegramService.PinChatMessageAsync(request.ReplyToMessage.MessageId);

        return await _telegramService.SendMessageAsync("📌 Pesan berhasil dipin");
    }
}