using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class CreateWebSessionRequestModel : RequestBase
{
}

public class CreateWebSessionRequestHandler : IRequestHandler<CreateWebSessionRequestModel, ResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly UserDbContext _userDbContext;

    public CreateWebSessionRequestHandler(TelegramService telegramService, UserDbContext userDbContext)
    {
        _telegramService = telegramService;
        _userDbContext = userDbContext;
    }

    public async Task<ResponseBase> Handle(CreateWebSessionRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var sessionId = Guid.NewGuid().ToString();
        var webUrlBase = Env.WEB_VERIFY_SESSION_URL + "?sessionId=";
        var webUrl = webUrlBase + sessionId;

        if (!EnvUtil.IsEnvExist(Env.WEB_CONSOLE_URL))
        {
            await _telegramService.SendMessageText("Maaf fitur ini belum dipersiapkan");
        }

        var htmlMessage = HtmlMessage.Empty
            .BoldBr("🎛 ZiziBot Console")
            .TextBr("Silakan klik tombol dibawah ini untuk membuka.")
            .Br();

        if (webUrl.Contains("localhost"))
        {
            htmlMessage.Code(webUrl).Br();
        }

        var replyMarkup = InlineKeyboardMarkup.Empty();
        if (!webUrl.Contains("localhost"))
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

        return await _telegramService.SendMessageText(htmlMessage.ToString(), replyMarkup);
    }
}