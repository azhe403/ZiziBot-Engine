using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class PingBotRequestModel : BotRequestBase
{ }

public class PingRequestHandler(
    ServiceFacade serviceFacade,
    DataFacade dataFacade
) : IBotRequestHandler<PingBotRequestModel>
{
    public async Task<BotResponseBase> Handle(PingBotRequestModel request, CancellationToken cancellationToken)
    {
        serviceFacade.TelegramService.SetupResponse(request);

        var htmlMessage = HtmlMessage.Empty
            .BoldBr("Pong!")
            .Br();

        var replyMarkup = InlineKeyboardMarkup.Empty();

        if (await dataFacade.ChatSetting.IsSudoAsync(request.UserId))
        {
            // replyMarkup = new InlineKeyboardMarkup(
            //     new[] {
            //         new[] {
            //             new InlineKeyboardButton("WebHook Info") {
            //                 CallbackData = new PingCallbackQueryModel() {
            //                     Path = CallbackConst.BOT,
            //                     Data = "webhook-info"
            //                 }
            //             }
            //         }
            //     }
            // );
        }

        return await serviceFacade.TelegramService.SendMessageAsync(htmlMessage.ToString(), replyMarkup);
    }
}