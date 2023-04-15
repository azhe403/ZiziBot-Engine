using MongoFramework.Linq;

namespace ZiziBot.DataSource.Repository;

public class AppSettingRepository
{
    private readonly AppSettingsDbContext _appSettingsDbContext;

    public AppSettingRepository(AppSettingsDbContext appSettingsDbContext)
    {
        _appSettingsDbContext = appSettingsDbContext;
    }

    public TelegramSinkConfigDto GetTelegramSinkConfig()
    {
        var chatId = _appSettingsDbContext.AppSettings.FirstOrDefault(entity => entity.Name == "EventLog:ChatId")?.Value;
        var threadId = _appSettingsDbContext.AppSettings.FirstOrDefault(entity => entity.Name == "EventLog:ThreadId")?.Value;
        var botToken = _appSettingsDbContext.BotSettings.FirstOrDefault(entity => entity.Name == "Main")?.Token;

        return new TelegramSinkConfigDto()
        {
            BotToken = botToken,
            ChatId = chatId.Convert<long>(),
            ThreadId = threadId.Convert<long>()
        };
    }

    public async Task<BotSettingsEntity> GetBotMain()
    {
        var botSetting = await _appSettingsDbContext.BotSettings
            .Where(entity => entity.Name == "Main")
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        return botSetting;
    }
}