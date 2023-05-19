using Telegram.Bot;

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
        var config = await _appSettingRepository.GetConfigSectionAsync<EngineConfig>();

        if (config != null)
        {
            htmlMessage
                .Bold(config.ProductName).Br()
                .Text("by ").Text(config.Vendor).Br()
                .Text(config.Description).Br().Br();
        }

        htmlMessage
            .Bold("Version: ").Text(VersionUtil.GetVersion(true)).Text($" (").Code(VersionUtil.GetVersion()).Text(")").Br()
            .Bold("Build Date: ").Code(VersionUtil.GetBuildDate().ToString("u")).Br();

        return await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}