using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class GetAboutBotRequest : BotRequestBase
{
}

public class GetAboutHandler : IRequestHandler<GetAboutBotRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly AppSettingRepository _appSettingRepository;

    public GetAboutHandler(TelegramService telegramService, AppSettingRepository appSettingRepository)
    {
        _telegramService = telegramService;
        _appSettingRepository = appSettingRepository;
    }

    public async Task<BotResponseBase> Handle(GetAboutBotRequest request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        _telegramService.SetupResponse(request);

        var me = await _telegramService.Bot.GetMeAsync(cancellationToken: cancellationToken);
        var botFullMention = me.GetFullMention();

        var config = await _appSettingRepository.GetConfigSectionAsync<EngineConfig>();

        var versionNumber = VersionUtil.GetVersionNumber();

        if (config != null)
        {
            htmlMessage
                .Bold(config.ProductName).Bold($" Build {versionNumber.Build} (").Code(VersionUtil.GetVersion()).Bold(")").Br()
                .Bold("Build Date: ").Code(VersionUtil.GetBuildDate().ToString("u")).Br()
                .Text("by ").Text(config.Vendor).Br()
                .Br();
        }

        htmlMessage.Text("Adalah bot pendebug dan manajemen grup. Ditulis menggunakan .NET (C#). ").Br().Br();
        htmlMessage.Text("Silakan bergabung ke channel <b>WinTen Dev</b> untuk mendapatkan informasi terbaru mengenai pembaruan. " +
                         "Untuk detail fitur pada perintah /start.").Br().Br();

        htmlMessage.Text($"Untuk <b>{botFullMention}</b> agar lebih cepat dan tetap cepat dan terus peningkatan dan keandalan, silakan <b>Sawer</b>/<b>Trakteer</b> untuk biaya Server dan Kopi 🍵.").Br().Br();

        htmlMessage.Text("Terima kasih kepada <b>Akmal Projext</b> yang telah memberikan kesempatan <b>Zizi Bot</b> pada kehidupan sebelumnya.");

        var replyMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("🪟 WinTen Group", "https://t.me/WinTenGroup"),
                InlineKeyboardButton.WithUrl("❤️ WinTen Dev", "https://t.me/WinTenDev"),
            },
            new[]
            {
                InlineKeyboardButton.WithUrl("📱 Redmi 5A (ID)", "https://t.me/Redmi5AID"),
                InlineKeyboardButton.WithUrl("🤖 Android APK", "https://t.me/ApkFreeID"),
            },
            new[]
            {
                InlineKeyboardButton.WithUrl("😺 Telegram Bot API", "https://t.me/TgBotID"),
                InlineKeyboardButton.WithUrl("🏗 Akmal Projext", "https://t.me/AkmalProjext"),
            },
            new[]
            {
                InlineKeyboardButton.WithUrl("🥛 Trakteer", "https://trakteer.id/azhe403/tip"),
                InlineKeyboardButton.WithUrl("🫲 Saweria", "https://saweria.co/azhe403"),
            }
        });

        return await _telegramService.SendMessageText(htmlMessage.ToString(), replyMarkup);
    }
}