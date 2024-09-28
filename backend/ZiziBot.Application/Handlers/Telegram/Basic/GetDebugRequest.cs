namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class GetDebugBotRequest : BotRequestBase
{
}

public class GetDebugHandler(TelegramService telegramService) : IBotRequestHandler<GetDebugBotRequest>
{
    public async Task<BotResponseBase> Handle(GetDebugBotRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        var htmlMessage = HtmlMessage.Empty
            .BoldBr("Debug Request")
            .Br();

        var message = request.Message;

        switch (telegramService.GetCommand())
        {
            case "/dbg":
                htmlMessage.CodeBr(message.ToYaml().HtmlDecode());
                break;
            case "/json":
                htmlMessage.CodeBr(message.ToJson(true).HtmlDecode());
                break;
        }

        return await telegramService.SendMessageText(htmlMessage.ToString());
    }
}