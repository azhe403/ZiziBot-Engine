using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ZiziBot.DataSource.MongoDb.Entities;
using File = System.IO.File;

namespace ZiziBot.Application.Handlers.Telegram.Data;

public class MongoDbBackupRequest : IRequest<bool>
{
}

public class MongoDbBackupHandler(
    ILogger<MongoDbBackupHandler> logger,
    MongoDbContextBase mongoDbContext,
    AppSettingRepository appSettingRepository)
    : IRequestHandler<MongoDbBackupRequest, bool>
{
    public async Task<bool> Handle(MongoDbBackupRequest request, CancellationToken cancellationToken)
    {
        var botMain = await appSettingRepository.GetBotMain();

        var config = await appSettingRepository.GetConfigSectionAsync<EventLogConfig>();

        if (config == null)
        {
            logger.LogWarning("Event Log seem not configured yet");
            return false;
        }

        var exportPath = PathConst.MONGODB_BACKUP.EnsureDirectory();

        await mongoDbContext.ExportAllAsync<AppSettingsEntity>();
        await mongoDbContext.ExportAllAsync<ApiKeyEntity>();
        await mongoDbContext.ExportAllAsync<BotCommandEntity>();
        await mongoDbContext.ExportAllAsync<BotSettingsEntity>();
        await mongoDbContext.ExportAllAsync<BangHasan_ShalatCityEntity>();
        await mongoDbContext.ExportAllAsync<ChannelMapEntity>();
        await mongoDbContext.ExportAllAsync<ChannelPostEntity>();
        // await _mirrorDbContext.ExportAllAsync<BinderByteCheckAwbEntity>();
        await mongoDbContext.ExportAllAsync<ChatSettingEntity>();
        await mongoDbContext.ExportAllAsync<GlobalBanEntity>();
        await mongoDbContext.ExportAllAsync<GroupTopicEntity>();
        await mongoDbContext.ExportAllAsync<MirrorApprovalEntity>();
        await mongoDbContext.ExportAllAsync<MirrorUserEntity>();
        await mongoDbContext.ExportAllAsync<NoteEntity>();
        await mongoDbContext.ExportAllAsync<RssSettingEntity>();
        await mongoDbContext.ExportAllAsync<SudoerEntity>();
        // await _mirrorDbContext.ExportAllAsync<TonjooAwbEntity>();
        await mongoDbContext.ExportAllAsync<WebhookChatEntity>();
        await mongoDbContext.ExportAllAsync<WelcomeMessageDto>();
        await mongoDbContext.ExportAllAsync<WordFilterEntity>();

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

        PathConst.BACKUP
            .GetFiles(pattern: "MongoDB*zip", predicate: x => DateTime.UtcNow.AddMonths(-2) > x.CreationTime)
            .DeleteFile();

        return true;
    }
}