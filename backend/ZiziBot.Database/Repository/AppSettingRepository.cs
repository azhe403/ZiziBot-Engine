using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZiziBot.Common.Dtos.Entity;
using ZiziBot.Common.Interfaces;
using ZiziBot.Common.Utils;
using ZiziBot.Database.MongoDb;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Database.Repository;

public class AppSettingRepository(
    MongoDbContext mongoDbContext,
    IServiceProvider serviceProvider
    )
{
    private ICacheService CacheService => serviceProvider.GetRequiredService<ICacheService>();

    public TelegramSinkConfigDto GetTelegramSinkConfig()
    {
        var botToken = mongoDbContext.BotSettings.Select(x => new { x.Name, x.Token }).FirstOrDefault(entity => entity.Name == "Main");

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
        var appSettings = mongoDbContext.AppSettings.AsNoTracking()
            .Where(entity => entity.Root == rootConfig)
            .ToList();

        return appSettings;
    }

    private async Task<List<AppSettingsEntity>> GetConfigSectionInternalAsync(string rootConfig)
    {
        var appSettings = await mongoDbContext.AppSettings.AsNoTracking()
            .Where(entity => entity.Root == rootConfig)
            .ToListAsync();

        return appSettings;
    }

    public async Task UpdateAppSetting(string name, string value)
    {
        var appSettings = await mongoDbContext.AppSettings
            .Where(entity => entity.Name == name)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (appSettings == null)
            return;

        appSettings.Value = value;

        await mongoDbContext.SaveChangesAsync();
    }

    public async Task<BotSettingDto> GetBotMain()
    {
        var cached = await CacheService.GetOrSetAsync(CacheKey.GLOBAL_BOT_MAIN, async () => {
            var botSetting = await mongoDbContext.BotSettings.AsNoTracking()
                .Where(entity => entity.Name == "Main")
                .Where(entity => entity.Status == EventStatus.Complete)
                .Select(x => new BotSettingDto {
                    Name = x.Name,
                    Token = x.Token
                })
                .FirstOrDefaultAsync();

            ArgumentNullException.ThrowIfNull(botSetting);

            return botSetting;
        });

        return cached;
    }


    public async Task<List<BotSettingDto>> ListBots()
    {
        var listBotData = await mongoDbContext.BotSettings
            .Where(settings => settings.Status == EventStatus.Complete)
            .Select(x => new BotSettingDto {
                Name = x.Name,
                Token = x.Token
            })
            .ToListAsync();

        return listBotData;
    }

    public async Task<ApiKeyEntity> GetRequiredApiKeyAsync(ApiKeyCategory category, ApiKeyVendor name)
    {
        var apiKey = await GetApiKeyAsync(category, name);

        return apiKey ?? throw new InvalidOperationException("Api key not found");
    }

    public async Task<ApiKeyEntity?> GetApiKeyAsync(ApiKeyCategory category, ApiKeyVendor name)
    {
        var query = mongoDbContext.ApiKey.AsNoTracking();

        if (name == ApiKeyVendor.GitHub)
            query = query.OrderByDescending(x => x.Remaining);
        else
            query = query.OrderBy(entity => entity.LastUsedDate);

        query = query.Where(entity => entity.Status == EventStatus.Complete)
            .Where(entity => entity.Category == category)
            .Where(entity => entity.Name == name);

        var apiKey = await query.FirstOrDefaultAsync();

        if (Env.GithubToken.IsNullOrWhiteSpace() &&
            apiKey is { Name: ApiKeyVendor.GitHub, Remaining: > 0 })
            Env.GithubToken = apiKey.ApiKey;

        return apiKey;
    }
}