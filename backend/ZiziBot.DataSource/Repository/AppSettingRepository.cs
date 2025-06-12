using Microsoft.EntityFrameworkCore;
using ZiziBot.Contracts.Dtos.Entity;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.DataSource.Repository;

public class AppSettingRepository(MongoEfContext mongoEfContext)
{
    public TelegramSinkConfigDto GetTelegramSinkConfig()
    {
        var botToken = mongoEfContext.BotSettings.Select(x => new { x.Name, x.Token }).FirstOrDefault(entity => entity.Name == "Main");

        var eventLogConfig = GetConfigSection<EventLogConfig>();

        return new TelegramSinkConfigDto() {
            BotToken = botToken?.Token,
            ChatId = eventLogConfig?.ChatId,
            ThreadId = eventLogConfig?.ThreadId
        };
    }

    public T GetRequiredConfigSection<T>() where T : new()
    {
        var config = GetConfigSection<T>();

        return config ?? throw new InvalidOperationException("Config section not found");
    }

    public async Task<T> GetRequiredConfigSectionAsync<T>() where T : new()
    {
        var config = await GetConfigSectionAsync<T>();

        return config ?? throw new InvalidOperationException("Config section not found");
    }

    public async Task<T?> GetConfigSectionAsync<T>() where T : new()
    {
        var rootConfig = typeof(T).Name.Replace("Config", "");

        var appSettings = await GetConfigSectionInternalAsync(rootConfig);

        var data = appSettings
            .DistinctBy(d => d.Name)
            .ToDictionary(x => x.Name.Remove(0, rootConfig.Length + 1), x => x.Value).ToJson().ToObject<T>();

        return data;
    }

    public T? GetConfigSection<T>() where T : new()
    {
        var rootConfig = typeof(T).Name.Replace("Config", "");

        var appSettings = GetConfigSectionInternal(rootConfig);

        var data = appSettings
            .DistinctBy(d => d.Name)
            .ToDictionary(x => x.Name.Remove(0, rootConfig.Length + 1), x => x.Value).ToJson().ToObject<T>();

        return data;
    }

    private List<AppSettingsEntity> GetConfigSectionInternal(string rootConfig)
    {
        var appSettings = mongoEfContext.AppSettings.AsNoTracking()
            .Where(entity => entity.Root == rootConfig)
            .ToList();

        return appSettings;
    }

    private async Task<List<AppSettingsEntity>> GetConfigSectionInternalAsync(string rootConfig)
    {
        var appSettings = await mongoEfContext.AppSettings.AsNoTracking()
            .Where(entity => entity.Root == rootConfig)
            .ToListAsync();

        return appSettings;
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
        var listBotData = await mongoEfContext.BotSettings
            .Where(settings => settings.Status == EventStatus.Complete)
            .Select(x => new BotSettingDto {
                Name = x.Name,
                Token = x.Token
            })
            .ToListAsync();

        return listBotData;
    }

    public async Task<string> GetRequiredApiKeyAsync(ApiKeyCategory category, ApiKeyVendor name)
    {
        var apiKey = await GetApiKeyAsync(category, name);

        return apiKey ?? throw new InvalidOperationException("Api key not found");
    }

    public async Task<string> GetApiKeyAsync(ApiKeyCategory category, ApiKeyVendor name)
    {
        var apiKey = await mongoEfContext.ApiKey
            .OrderBy(entity => entity.LastUsedDate)
            .Where(entity => entity.Category == category)
            .Where(entity => entity.Name == name)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (apiKey == null)
            return string.Empty;

        apiKey.LastUsedDate = DateTime.UtcNow;
        apiKey.TransactionId = Guid.NewGuid().ToString();
        await mongoEfContext.SaveChangesAsync();

        return apiKey.ApiKey;
    }
}