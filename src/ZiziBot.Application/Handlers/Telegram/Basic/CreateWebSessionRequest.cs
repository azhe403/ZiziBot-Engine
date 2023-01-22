using MongoFramework.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class CreateWebSessionRequestModel : RequestBase
{
}

public class CreateWebSessionRequestHandler : IRequestHandler<CreateWebSessionRequestModel, ResponseBase>
{
    private readonly UserDbContext _userDbContext;

    public CreateWebSessionRequestHandler(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task<ResponseBase> Handle(CreateWebSessionRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new(request);

        var dashboardSession = await _userDbContext.DashboardSessions
            .Where(
                session =>
                    session.TelegramUserId == request.UserId &&
                    session.Status == (int) EventStatus.Complete
            )
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var sessionId = Guid.NewGuid().ToString();
        var webUrl = Env.WEB_VERIFY_SESSION_URL + sessionId;

        if (dashboardSession != null)
        {
            webUrl = Env.WEB_VERIFY_SESSION_URL + dashboardSession.SessionId;
        }
        else
        {
            _userDbContext.DashboardSessions.Add(
                new DashboardSession()
                {
                    FirstName = request.UserFullName,
                    TelegramUserId = request.UserId,
                    SessionId = sessionId,
                    Status = (int) EventStatus.Complete
                }
            );

            await _userDbContext.SaveChangesAsync(cancellationToken);
        }

        var htmlMessage = HtmlMessage.Empty
            .TextBr("WebSession berhasil dibuat!")
            .TextBr("silakan klik tombol dibawah ini untuk membuka.")
            .Code(webUrl).Br().Br()
            .Bold("Catatan: ").Text("tautan ini hanya berfungsi untuk Anda!");

        var replyMarkup = InlineKeyboardMarkup.Empty();
        if (!webUrl.Contains("localhost"))
        {
            replyMarkup = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Open Web", webUrl));
        }

        return await responseBase.SendMessageText(htmlMessage.ToString(), replyMarkup);
    }
}