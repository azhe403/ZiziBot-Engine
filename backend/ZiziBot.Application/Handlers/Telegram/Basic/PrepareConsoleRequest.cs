using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class PrepareConsoleBotRequest : BotRequestBase
{
}

public class PrepareConsoleHandler(TelegramService telegramService) : IRequestHandler<PrepareConsoleBotRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(PrepareConsoleBotRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        var sessionId = Guid.NewGuid().ToString();
        var consoleUrl = EnvUtil.GetEnv(Env.WEB_CONSOLE_URL);
        var webUrlBase = consoleUrl + "?session_id=";
        var webUrl = webUrlBase + sessionId;

        if (!EnvUtil.IsEnvExist(Env.WEB_CONSOLE_URL))
        {
            await telegramService.SendMessageText("Maaf fitur ini belum dipersiapkan");
        }

        var replyMarkup = InlineKeyboardMarkup.Empty();
        var htmlMessage = HtmlMessage.Empty
            .BoldBr("🎛 ZiziBot Console")
            .TextBr("Buka Console untuk mengelola pengaturan, catatan dan lain-lain.")
            .Br();

        if (webUrl.Contains("localhost"))
        {
            htmlMessage.Code(webUrl).Br();
        }
        else
        {
            replyMarkup = new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithLoginUrl("Buka Console", new LoginUrl()
                    {
                        Url = webUrl
                    })
                }
            }.ToButtonMarkup();
        }

        return await telegramService.SendMessageText(htmlMessage.ToString(), replyMarkup);
    }
}