using Telegram.Bot;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class PingCallbackBotRequestModel : BotRequestBase
{
}

public class PingCallbackRequestHandler(SudoService sudoService, TelegramService telegramService) : IRequestHandler<PingCallbackBotRequestModel, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(PingCallbackBotRequestModel request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        if (!await sudoService.IsSudoAsync(request.UserId))
        {
            return await telegramService.AnswerCallbackAsync("Kamu tidak memiliki akses");
        }

        var webhookInfo = await telegramService.Bot.GetWebhookInfoAsync(cancellationToken: cancellationToken);

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

        await telegramService.AnswerCallbackAsync(messageCallback, showAlert: true);

        await telegramService.SendMessageText(htmlMessage.ToString(), chatId: request.UserId);

        return telegramService.Complete();
    }
}