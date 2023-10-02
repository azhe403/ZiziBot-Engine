namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class GetDebugBotRequest : BotRequestBase
{
}

public class GetDebugHandler : IBotRequestHandler<GetDebugBotRequest>
{
    private readonly TelegramService _telegramService;

    public GetDebugHandler(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<BotResponseBase> Handle(GetDebugBotRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var htmlMessage = HtmlMessage.Empty
            .BoldBr("Debug Request")
            .Br();

        var message = request.Message;

        switch (_telegramService.GetCommand())
        {
            case "/dbg":
                htmlMessage.CodeBr(message.ToYaml().HtmlDecode());
                break;
            case "/json":
                htmlMessage.CodeBr(message.ToJson(true).HtmlDecode());
                break;
        }

        return await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}