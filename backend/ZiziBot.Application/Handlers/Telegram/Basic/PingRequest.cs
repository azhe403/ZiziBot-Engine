using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class PingBotRequestModel : BotRequestBase
{
}

public class PingRequestHandler(SudoService sudoService, TelegramService telegramService) : IRequestHandler<PingBotRequestModel, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(PingBotRequestModel request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        var htmlMessage = HtmlMessage.Empty
            .BoldBr("Pong!")
            .Br();

        var replyMarkup = InlineKeyboardMarkup.Empty();

        if (await sudoService.IsSudoAsync(request.UserId))
        {
            replyMarkup = new InlineKeyboardMarkup(
                new[] {
                    new[] {
                        new InlineKeyboardButton("WebHook Info") {
                            CallbackData = new PingCallbackQueryModel() {
                                Path = CallbackConst.BOT,
                                Data = "webhook-info"
                            }
                        }
                    }
                }
            );
        }

        return await telegramService.SendMessageText(htmlMessage.ToString(), replyMarkup);
    }
}