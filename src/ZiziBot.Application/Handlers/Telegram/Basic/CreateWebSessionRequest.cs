using MongoFramework.Linq;
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

        var dashboardSession = await _userDbContext.DashboardSessions
            .Where(
                session =>
                    session.TelegramUserId == request.UserId &&
                    session.Status == (int)EventStatus.Complete
            )
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var sessionId = Guid.NewGuid().ToString();
        var webUrlBase = Env.WEB_VERIFY_SESSION_URL + "?sessionId=";
        var webUrl = webUrlBase + sessionId;

        if (!EnvUtil.IsEnvExist(Env.WEB_CONSOLE_URL))
        {
            await _telegramService.SendMessageText("Maaf fitur ini belum dipersiapkan");
        }

        if (dashboardSession != null)
        {
            webUrl = webUrlBase + dashboardSession.SessionId;
        }
        else
        {
            _userDbContext.DashboardSessions.Add(
                new DashboardSession()
                {
                    FirstName = request.UserFullName,
                    TelegramUserId = request.UserId,
                    SessionId = sessionId,
                    Status = (int)EventStatus.Complete
                }
            );

            await _userDbContext.SaveChangesAsync(cancellationToken);
        }

        var htmlMessage = HtmlMessage.Empty
            .TextBr("WebSession berhasil dibuat!")
            .TextBr("silakan klik tombol dibawah ini untuk membuka.")
            .Br();

        if (webUrl.Contains("localhost"))
        {
            htmlMessage.Code(webUrl).Br();
        }

        htmlMessage.Bold("Catatan: ").Text("Tombol ini hanya berfungsi untuk Anda!");

        var replyMarkup = InlineKeyboardMarkup.Empty();
        if (!webUrl.Contains("localhost"))
        {
            replyMarkup = new InlineKeyboardMarkup(InlineKeyboardButton.WithLoginUrl("Open Web", new LoginUrl()
            {
                Url = webUrl
            }));
        }

        return await _telegramService.SendMessageText(htmlMessage.ToString(), replyMarkup);
    }
}