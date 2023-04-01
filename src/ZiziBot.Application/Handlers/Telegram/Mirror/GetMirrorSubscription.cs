using MongoFramework.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Mirror;

public class GetMirrorSubscriptionRequest : RequestBase
{

}

public class GetMirrorSubscriptionHandler : IRequestHandler<GetMirrorSubscriptionRequest, ResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly MirrorDbContext _mirrorDbContext;

    public GetMirrorSubscriptionHandler(TelegramService telegramService, MirrorDbContext mirrorDbContext)
    {
        _telegramService = telegramService;
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<ResponseBase> Handle(GetMirrorSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        _telegramService.SetupResponse(request);

        var mirrorSubscription = await _mirrorDbContext.MirrorUsers
            .FirstOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken);

        var replyMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("Mengapa donasi?", "https://docs.mirror.winten.my.id/donasi")
            },
            new[]
            {
                InlineKeyboardButton.WithUrl("Donasi", "https://t.me/ContactWinTenBot?start=donate"),
                InlineKeyboardButton.WithUrl("Donate", "https://t.me/ContactWinTenBot?start=donate")
            }
        });

        if (mirrorSubscription == null)
        {
            htmlMessage.Bold("Anda belum berlangganan Mirror").Br()
                .Text("Silahkan Donasi untuk mendapatkan akses mirror").Br();

            return await _telegramService.SendMessageText(text: htmlMessage.ToString(), replyMarkup: replyMarkup);
        }

        htmlMessage.BoldBr("🪞 Langganan Mirror")
            .Bold("🏷 Nama: ").CodeBr(request.UserFullName)
            .Bold("🆔 ID Pengguna: ").CodeBr(mirrorSubscription.UserId.ToString())
            .Bold("📅 Kedaluarsa: ").CodeBr(mirrorSubscription.ExpireDate.ToString("yyyy-MM-dd HH:mm:ss"))
            .Bold("⏱ Sejak: ").CodeBr(mirrorSubscription.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"))
            .Bold("⏳ Durasi: ").CodeBr(mirrorSubscription.Duration.ForHuman(4));

        return await _telegramService.SendMessageText(text: htmlMessage.ToString(), replyMarkup: replyMarkup);
    }
}