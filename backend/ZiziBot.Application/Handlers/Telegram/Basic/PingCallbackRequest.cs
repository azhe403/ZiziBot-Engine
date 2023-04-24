using Telegram.Bot;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class PingCallbackRequestModel : RequestBase
{
}

public class PingCallbackRequestHandler : IRequestHandler<PingCallbackRequestModel, ResponseBase>
{
    private readonly SudoService _sudoService;
    private readonly TelegramService _telegramService;

    public PingCallbackRequestHandler(SudoService sudoService, TelegramService telegramService)
    {
        _sudoService = sudoService;
        _telegramService = telegramService;
    }

    public async Task<ResponseBase> Handle(PingCallbackRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        if (!await _sudoService.IsSudoAsync(request.UserId))
        {
            return await _telegramService.AnswerCallbackAsync("Kamu tidak memiliki akses");
        }

        var webhookInfo = await _telegramService.Bot.GetWebhookInfoAsync(cancellationToken: cancellationToken);

        var htmlMessage = HtmlMessage.Empty;

        if (!webhookInfo.Url.IsNullOrEmpty())
        {
            htmlMessage.Text("URL: ").TextBr(webhookInfo.Url);
        }
        else
        {
            htmlMessage.Text("Engine mode bukan Webhook");
        }

        return await _telegramService.AnswerCallbackAsync(htmlMessage.ToString(), showAlert: true);
    }
}