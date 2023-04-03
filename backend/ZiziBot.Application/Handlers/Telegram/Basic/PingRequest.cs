using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class PingRequestModel : RequestBase
{
}

public class PingRequestHandler : IRequestHandler<PingRequestModel, ResponseBase>
{
    private readonly SudoService _sudoService;
    private readonly TelegramService _telegramService;

    public PingRequestHandler(SudoService sudoService, TelegramService telegramService)
    {
        _sudoService = sudoService;
        _telegramService = telegramService;
    }

    public async Task<ResponseBase> Handle(PingRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var htmlMessage = HtmlMessage.Empty
            .BoldBr("Pong!")
            .Br();

        var replyMarkup = InlineKeyboardMarkup.Empty();

        if (await _sudoService.IsSudoAsync(request.UserId))
        {
            replyMarkup = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        new InlineKeyboardButton("WebHook Info")
                        {
                            CallbackData = new PingCallbackQueryModel()
                            {
                                Path = CallbackConst.BOT,
                                Data = "webhook-info"
                            }
                        }
                    }
                }
            );
        }

        return await _telegramService.SendMessageText(htmlMessage.ToString(), replyMarkup);
    }
}