using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class PingRequestModel : RequestBase
{
}

public class PingRequestHandler : IRequestHandler<PingRequestModel, ResponseBase>
{
    private readonly SudoService _sudoService;

    public PingRequestHandler(SudoService sudoService)
    {
        _sudoService = sudoService;
    }

    public async Task<ResponseBase> Handle(PingRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new(request);

        var webhookInfo = await responseBase.Bot.GetWebhookInfoAsync(cancellationToken: cancellationToken);

        var htmlMessage = HtmlMessage.Empty
            .BoldBr("Pong!")
            .Br();

        var replyMarkup = InlineKeyboardMarkup.Empty();

        if (await _sudoService.IsSudoAsync(request.UserId) &&
            webhookInfo.Url != null)
        {
            replyMarkup = new InlineKeyboardMarkup(new[]
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
            });
        }

        return await responseBase.SendMessageText(htmlMessage.ToString(), replyMarkup);
    }
}