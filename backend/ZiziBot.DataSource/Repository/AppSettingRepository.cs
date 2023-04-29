using System.ComponentModel;
using System.Reflection;
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


    public async Task<T?> GetConfigSection<T>() where T : new()
    {
        var attribute = typeof(T).GetCustomAttribute<DisplayNameAttribute>();
        if (attribute == null)
        {
            throw new ArgumentException("T must have DisplayName Attribute");
        }

        var sectionName = attribute.DisplayName;

        var appSettings = await _appSettingsDbContext.AppSettings
            .Where(entity => entity.Name.StartsWith(sectionName))
            .Select(x => new { x.Name, x.Value })
            .ToListAsync();

        var data = appSettings
            .DistinctBy(d => d.Name)
            .ToDictionary(x => x.Name.Remove(0, sectionName.Length + 1), x => x.Value).ToJson().ToObject<T>();

        return data;
    }

    public async Task UpdateAppSetting(string name, string value)
    {
        var appSettings = await _appSettingsDbContext.AppSettings
            .Where(entity => entity.Name == name)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        appSettings.Value = value;

        await _appSettingsDbContext.SaveChangesAsync();
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