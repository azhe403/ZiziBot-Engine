using Telegram.Bot;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class PingCallbackBotRequestModel : BotRequestBase
{ }

public class PingCallbackRequestHandler(
    ServiceFacade serviceFacade,
    DataFacade dataFacade
) : IBotRequestHandler<PingCallbackBotRequestModel>
{
    public async Task<BotResponseBase> Handle(PingCallbackBotRequestModel request, CancellationToken cancellationToken)
    {
        serviceFacade.TelegramService.SetupResponse(request);

        if (!await dataFacade.ChatSetting.IsSudoAsync(request.UserId))
        {
            return await serviceFacade.TelegramService.AnswerCallbackAsync("Pong!");
        }

        var webhookInfo = await serviceFacade.TelegramService.Bot.GetWebhookInfo(cancellationToken: cancellationToken);

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

        await serviceFacade.TelegramService.AnswerCallbackAsync(messageCallback, showAlert: true);

        return await serviceFacade.TelegramService.SendMessageText(htmlMessage.ToString(), chatId: request.UserId);
    }
}