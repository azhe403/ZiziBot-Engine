using MongoFramework.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Mirror;

public class GetMirrorSubscriptionBotRequest : BotRequestBase
{

}

public class GetMirrorSubscriptionHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext) : IRequestHandler<GetMirrorSubscriptionBotRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(GetMirrorSubscriptionBotRequest request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        telegramService.SetupResponse(request);

        var mirrorSubscription = await mongoDbContext.MirrorUsers
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

            return await telegramService.SendMessageText(text: htmlMessage.ToString(), replyMarkup: replyMarkup);
        }

        htmlMessage.BoldBr("🪞 Langganan Mirror")
            .Bold("🏷 Nama: ").CodeBr(request.UserFullName)
            .Bold("🆔 ID Pengguna: ").CodeBr(mirrorSubscription.UserId.ToString())
            .Bold("📅 Kedaluarsa: ").CodeBr(mirrorSubscription.ExpireDate.ToString("yyyy-MM-dd HH:mm:ss"))
            .Bold("⏱ Sejak: ").CodeBr(mirrorSubscription.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"))
            .Bold("⏳ Durasi: ").CodeBr(mirrorSubscription.Duration.ForHuman(4));

        return await telegramService.SendMessageText(text: htmlMessage.ToString(), replyMarkup: replyMarkup);
    }
}