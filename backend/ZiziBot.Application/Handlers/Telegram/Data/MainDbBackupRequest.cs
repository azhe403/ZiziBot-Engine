using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace ZiziBot.Application.Handlers.Telegram.Data;

public class MainDbBackupRequest : IRequest<bool>
{

}

public class MainDbBackupHandler : IRequestHandler<MainDbBackupRequest, bool>
{
    private readonly ILogger<MainDbBackupHandler> _logger;
    private readonly MirrorDbContext _mirrorDbContext;
    private readonly AppSettingRepository _appSettingRepository;

    public MainDbBackupHandler(ILogger<MainDbBackupHandler> logger, MirrorDbContext mirrorDbContext, AppSettingRepository appSettingRepository)
    {
        _logger = logger;
        _mirrorDbContext = mirrorDbContext;
        _appSettingRepository = appSettingRepository;
    }

    public async Task<bool> Handle(MainDbBackupRequest request, CancellationToken cancellationToken)
    {
        var botMain = await _appSettingRepository.GetBotMain();

        var config = await _appSettingRepository.GetConfigSectionAsync<EventLogConfig>();

        if (config == null)
        {
            _logger.LogWarning("Event Log seem not configured yet");
            return false;
        }

        await _mirrorDbContext.ExportAllAsync<AppSettingsEntity>();
        await _mirrorDbContext.ExportAllAsync<ApiKeyEntity>();
        await _mirrorDbContext.ExportAllAsync<BotCommandEntity>();
        await _mirrorDbContext.ExportAllAsync<BotSettingsEntity>();
        await _mirrorDbContext.ExportAllAsync<ChatSettingEntity>();
        await _mirrorDbContext.ExportAllAsync<GlobalBanEntity>();
        // await _mirrorDbContext.ExportAll<MirrorApprovalEntity>();
        await _mirrorDbContext.ExportAllAsync<MirrorUserEntity>();
        await _mirrorDbContext.ExportAllAsync<NoteEntity>();
        await _mirrorDbContext.ExportAllAsync<RssSettingEntity>();
        await _mirrorDbContext.ExportAllAsync<SudoerEntity>();
        await _mirrorDbContext.ExportAllAsync<WebhookChatEntity>();
        await _mirrorDbContext.ExportAllAsync<WelcomeMessageDto>();

        var date = DateTime.UtcNow.ToString("yyyy-MM-dd");

        var zipFile = PathConst.MONGODB_BACKUP.CompressToZip($"MongoDB-{date}.zip");

        var htmlMessage = HtmlMessage.Empty;

        var bot = new TelegramBotClient(botMain.Token);
        htmlMessage.Bold("Date: ").CodeBr(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss zzz"));

        await using var fileStream = File.OpenRead(path: zipFile);
        var inputOnlineFile = InputFile.FromStream(stream: fileStream, fileName: zipFile.GetFileName());

        await bot.SendDocumentAsync(
            chatId: config.ChatId,
            document: inputOnlineFile,
            caption: htmlMessage.ToString(),
            parseMode: ParseMode.Html,
            messageThreadId: config.BackupDB,
            cancellationToken: cancellationToken
        );

        return true;
    }
}