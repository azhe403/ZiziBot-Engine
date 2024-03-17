using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace ZiziBot.Application.Handlers.Telegram.Data;

public class MongoDbBackupRequest : IRequest<bool>
{
}

public class MongoDbBackupHandler : IRequestHandler<MongoDbBackupRequest, bool>
{
    private readonly ILogger<MongoDbBackupHandler> _logger;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly AppSettingRepository _appSettingRepository;

    public MongoDbBackupHandler(ILogger<MongoDbBackupHandler> logger, MongoDbContextBase mongoDbContext, AppSettingRepository appSettingRepository)
    {
        _logger = logger;
        _mongoDbContext = mongoDbContext;
        _appSettingRepository = appSettingRepository;
    }

    public async Task<bool> Handle(MongoDbBackupRequest request, CancellationToken cancellationToken)
    {
        var botMain = await _appSettingRepository.GetBotMain();

        var config = await _appSettingRepository.GetConfigSectionAsync<EventLogConfig>();

        if (config == null)
        {
            _logger.LogWarning("Event Log seem not configured yet");
            return false;
        }

        var exportPath = PathConst.MONGODB_BACKUP.EnsureDirectory();

        await _mongoDbContext.ExportAllAsync<AppSettingsEntity>();
        await _mongoDbContext.ExportAllAsync<ApiKeyEntity>();
        await _mongoDbContext.ExportAllAsync<BotCommandEntity>();
        await _mongoDbContext.ExportAllAsync<BotSettingsEntity>();
        await _mongoDbContext.ExportAllAsync<BangHasan_ShalatCityEntity>();
        await _mongoDbContext.ExportAllAsync<ChannelMapEntity>();
        await _mongoDbContext.ExportAllAsync<ChannelPostEntity>();
        // await _mirrorDbContext.ExportAllAsync<BinderByteCheckAwbEntity>();
        await _mongoDbContext.ExportAllAsync<ChatSettingEntity>();
        await _mongoDbContext.ExportAllAsync<GlobalBanEntity>();
        await _mongoDbContext.ExportAllAsync<GroupTopicEntity>();
        await _mongoDbContext.ExportAllAsync<MirrorApprovalEntity>();
        await _mongoDbContext.ExportAllAsync<MirrorUserEntity>();
        await _mongoDbContext.ExportAllAsync<NoteEntity>();
        await _mongoDbContext.ExportAllAsync<RssSettingEntity>();
        await _mongoDbContext.ExportAllAsync<SudoerEntity>();
        // await _mirrorDbContext.ExportAllAsync<TonjooAwbEntity>();
        await _mongoDbContext.ExportAllAsync<WebhookChatEntity>();
        await _mongoDbContext.ExportAllAsync<WelcomeMessageDto>();
        await _mongoDbContext.ExportAllAsync<WordFilterEntity>();

        var date = DateTime.UtcNow.ToString("yyyy-MM-dd");

        var zipFile = exportPath.CompressToZip($"MongoDB-{date}.zip");

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

        PathConst.BACKUP.GetFiles(pattern: "MongoDB*zip", predicate: x => DateTime.UtcNow.AddMonths(-2) > x.CreationTime).DeleteFile();

        return true;
    }
}