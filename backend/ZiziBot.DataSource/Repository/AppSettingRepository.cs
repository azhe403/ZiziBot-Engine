using Microsoft.EntityFrameworkCore;
using ZiziBot.Contracts.Dtos.Entity;
using ZiziBot.DataSource.MongoEf;

namespace ZiziBot.DataSource.Repository;

public class AppSettingRepository(MongoEfContext mongoEfContext)
{
    public TelegramSinkConfigDto GetTelegramSinkConfig()
    {
        var chatId = mongoEfContext.AppSettings.Select(x => new { x.Name, x.Value }).FirstOrDefault(entity => entity.Name == "EventLog:ChatId")?.Value;
        var threadId = mongoEfContext.AppSettings.Select(x => new { x.Name, x.Value }).FirstOrDefault(entity => entity.Name == "EventLog:ThreadId")?.Value;
        var botToken = mongoEfContext.BotSettings.Select(x => new { x.Name, x.Token }).FirstOrDefault(entity => entity.Name == "Main");

        var eventLogConfig = GetConfigSection<EventLogConfig>();

        return new TelegramSinkConfigDto() {
            BotToken = botToken?.Token,
            ChatId = chatId.Convert<long>(),
            ThreadId = eventLogConfig?.EventLog
        };
    }


    public async Task<T?> GetConfigSectionAsync<T>() where T : new()
    {
        var rootConfig = typeof(T).Name.Replace("Config", "");

        var appSettings = await mongoEfContext.AppSettings.AsNoTracking()
            .Where(entity => entity.Root == rootConfig)
            .Select(x => new { x.Name, x.Value })
            .ToListAsync();

        var data = appSettings
            .DistinctBy(d => d.Name)
            .ToDictionary(x => x.Name.Remove(0, rootConfig.Length + 1), x => x.Value).ToJson().ToObject<T>();

        return data;
    }

    public T? GetConfigSection<T>() where T : new()
    {
        var rootConfig = typeof(T).Name.Replace("Config", "");

        var appSettings = mongoEfContext.AppSettings.AsNoTracking()
            .Where(entity => entity.Root == rootConfig)
            .Select(x => new { x.Name, x.Value })
            .ToList();

        var data = appSettings
            .DistinctBy(d => d.Name)
            .ToDictionary(x => x.Name.Remove(0, rootConfig.Length + 1), x => x.Value).ToJson().ToObject<T>();

        return data;
    }

    public T GetRequiredConfigSection<T>() where T : new()
    {
        return GetConfigSection<T>() ?? new T();
    }


    public async Task UpdateAppSetting(string name, string value)
    {
        var appSettings = await mongoEfContext.AppSettings
            .Where(entity => entity.Name == name)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (appSettings == null)
            return;

        appSettings.Value = value;

        await mongoEfContext.SaveChangesAsync();
    }

    public async Task<BotSettingDto> GetBotMain()
    {
        var botSetting = await mongoEfContext.BotSettings.AsNoTracking()
            .Where(entity => entity.Name == "Main")
            .Where(entity => entity.Status == EventStatus.Complete)
            .Select(x => new BotSettingDto {
                Name = x.Name,
                Token = x.Token
            })
            .FirstOrDefaultAsync();

        ArgumentNullException.ThrowIfNull(botSetting);

        return botSetting;
    }


    public async Task<List<BotSettingDto>> ListBots()
    {
        var listBotData = mongoEfContext.BotSettings
            .Where(settings => settings.Status == EventStatus.Complete)
            .Select(x => new BotSettingDto {
                Name = x.Name,
                Token = x.Token
            })
            .ToList();

        return listBotData;
    }
}