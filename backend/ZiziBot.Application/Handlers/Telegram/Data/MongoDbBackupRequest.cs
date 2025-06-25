using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ZiziBot.Common.Types;
using ZiziBot.DataSource.MongoEf.Entities;
using File = System.IO.File;

namespace ZiziBot.Application.Handlers.Telegram.Data;

public class MongoDbBackupRequest : IRequest<bool>
{ }

public class MongoDbBackupHandler(
    ILogger<MongoDbBackupHandler> logger,
    DataFacade dataFacade
)
    : IRequestHandler<MongoDbBackupRequest, bool>
{
    public async Task<bool> Handle(MongoDbBackupRequest request, CancellationToken cancellationToken)
    {
        var botMain = await dataFacade.AppSetting.GetBotMain();

        var config = await dataFacade.AppSetting.GetConfigSectionAsync<EventLogConfig>();

        if (config == null)
        {
            logger.LogWarning("Event Log seem not configured yet");
            return false;
        }

        var exportPath = PathConst.MONGODB_BACKUP.EnsureDirectory();

        await dataFacade.MongoEf.ExportAllAsync<AppSettingsEntity>();
        await dataFacade.MongoEf.ExportAllAsync<ApiKeyEntity>();
        await dataFacade.MongoEf.ExportAllAsync<BotCommandEntity>();
        await dataFacade.MongoEf.ExportAllAsync<BotSettingsEntity>();
        await dataFacade.MongoEf.ExportAllAsync<BangHasan_ShalatCityEntity>();
        await dataFacade.MongoEf.ExportAllAsync<ChannelMapEntity>();
        await dataFacade.MongoEf.ExportAllAsync<ChannelPostEntity>();
        await dataFacade.MongoEf.ExportAllAsync<BinderByteCheckAwbEntity>();
        await dataFacade.MongoEf.ExportAllAsync<ChatSettingEntity>();
        await dataFacade.MongoEf.ExportAllAsync<GlobalBanEntity>();
        await dataFacade.MongoEf.ExportAllAsync<GroupTopicEntity>();
        await dataFacade.MongoEf.ExportAllAsync<MirrorApprovalEntity>();
        await dataFacade.MongoEf.ExportAllAsync<MirrorUserEntity>();
        await dataFacade.MongoEf.ExportAllAsync<NoteEntity>();
        await dataFacade.MongoEf.ExportAllAsync<RssSettingEntity>();
        await dataFacade.MongoEf.ExportAllAsync<SudoerEntity>();
        await dataFacade.MongoEf.ExportAllAsync<TonjooAwbEntity>();
        await dataFacade.MongoEf.ExportAllAsync<WebhookChatEntity>();
        await dataFacade.MongoEf.ExportAllAsync<WelcomeMessageEntity>();
        await dataFacade.MongoEf.ExportAllAsync<WordFilterEntity>();

        var date = DateTime.UtcNow.ToString("yyyy-MM-dd");

        var zipFile = exportPath.CompressToZip($"MongoDB-{date}.zip");

        var htmlMessage = HtmlMessage.Empty;

        var bot = new TelegramBotClient(botMain.Token);
        htmlMessage.Bold("Date: ").CodeBr(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss zzz"));

        await using var fileStream = File.OpenRead(path: zipFile);
        var inputOnlineFile = InputFile.FromStream(stream: fileStream, fileName: zipFile.GetFileName());

        await bot.SendDocument(
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