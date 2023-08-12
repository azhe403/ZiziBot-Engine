using Telegram.Bot;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class PingCallbackBotRequestModel : BotRequestBase
{
}

public class PingCallbackRequestHandler : IRequestHandler<PingCallbackBotRequestModel, BotResponseBase>
{
    private readonly SudoService _sudoService;
    private readonly TelegramService _telegramService;

    public PingCallbackRequestHandler(SudoService sudoService, TelegramService telegramService)
    {
        _sudoService = sudoService;
        _telegramService = telegramService;
    }

    public async Task<BotResponseBase> Handle(PingCallbackBotRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        if (!await _sudoService.IsSudoAsync(request.UserId))
        {
            return await _telegramService.AnswerCallbackAsync("Kamu tidak memiliki akses");
        }

        var webhookInfo = await _telegramService.Bot.GetWebhookInfoAsync(cancellationToken: cancellationToken);

        var messageCallback = string.Empty;
        var htmlMessage = HtmlMessage.Empty;

        if (!webhookInfo.Url.IsNullOrEmpty())
        {
            messageCallback = "WebHook info dikirimkan ke Private.";
        }
        else
        {
            messageCallback = "Engine mode bukan Webhook";
            htmlMessage.Text(messageCallback);
        }

        await _telegramService.AnswerCallbackAsync(messageCallback, showAlert: true);

        await _telegramService.SendMessageText(htmlMessage.ToString(), chatId: request.UserId);

        return _telegramService.Complete();
    }
}