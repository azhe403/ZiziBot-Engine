namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class PinMessageRequest : BotRequestBase
{
}

public class PinMessageHandler(TelegramService telegramService) : IBotRequestHandler<PinMessageRequest>
{
    public async Task<BotResponseBase> Handle(PinMessageRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        if (request.ReplyToMessage == null)
        {
            return await telegramService.SendMessageAsync("Spesifikan pesan yang ingin dipin..");
        }

        await telegramService.SendMessageAsync("📍 Sedang mengepin pesan..");

        await telegramService.PinChatMessageAsync(request.ReplyToMessage.MessageId);

        return await telegramService.SendMessageAsync("📌 Pesan berhasil dipin");
    }
}