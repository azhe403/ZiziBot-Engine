using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Services;

public class AntiSpamService
{
    private readonly ILogger<AntiSpamService> _logger;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly ApiKeyService _apiKeyService;
    private readonly CacheService _cacheService;

    private readonly string defaultStaleTime = "10m";

    public AntiSpamService(ILogger<AntiSpamService> logger, MongoDbContextBase mongoDbContext, ApiKeyService apiKeyService, CacheService cacheService)
    {
        _logger = logger;
        _mongoDbContext = mongoDbContext;
        _apiKeyService = apiKeyService;
        _cacheService = cacheService;
    }

    public async Task<AntiSpamDto> CheckSpamAsync(long chatId, long userId)
    {
        var antispamDto = new AntiSpamDto();

        if (userId == 0)
            return antispamDto;

        var taskCheckEss = CheckEssAsync(chatId, userId);
        var taskCheckCombotAntiSpam = CheckCombotAntiSpamAsync(chatId, userId);
        var taskCheckSpamWatch = CheckSpamWatchAntiSpamAsync(chatId, userId);
        await Task.WhenAll(taskCheckEss, taskCheckCombotAntiSpam, taskCheckSpamWatch);

        antispamDto.IsBanEss = await taskCheckEss;
        antispamDto.IsBanCasFed = (await taskCheckCombotAntiSpam).IsBanned;
        antispamDto.IsBanSwFed = (await taskCheckSpamWatch).IsBanned;

        antispamDto.CasRecord = (await taskCheckCombotAntiSpam).Result;
        antispamDto.SpamWatchRecord = (await taskCheckSpamWatch).BanRecord;

        antispamDto.IsBanAny = antispamDto.IsBanEss || antispamDto.IsBanCasFed || antispamDto.IsBanSwFed;

        return antispamDto;
    }

    private async Task<bool> CheckEssAsync(long chatId, long userId)
    {
        var cacheData = await _cacheService.GetOrSetAsync(
            cacheKey: CacheKey.BAN_ESS + userId,
            staleAfter: defaultStaleTime,
            action: async () => {
                var globalBanEntities = await _mongoDbContext.GlobalBan
                    .Where(entity => entity.UserId == userId && entity.Status == (int)EventStatus.Complete)
                    .ToListAsync();

                return globalBanEntities.Any();
            }
        );

        return cacheData;
    }

    private async Task<CombotAntispamApiDto> CheckCombotAntiSpamAsync(long chatId, long userId)
    {
        var cacheData = await _cacheService.GetOrSetAsync(
            cacheKey: CacheKey.BAN_CAS + userId,
            staleAfter: defaultStaleTime,
            action: async () => {
                var url = UrlConst.ANTISPAM_COMBOT_API.SetQueryParam("userId", userId);
                var antispamApiDto = await url.GetJsonAsync<CombotAntispamApiDto>();

                _logger.LogDebug("Combot AntiSpam for {UserId}: {@Dto}", userId, antispamApiDto);

                return antispamApiDto;
            }
        );

        return cacheData;
    }

    private async Task<SpamWatchResult> CheckSpamWatchAntiSpamAsync(long chatId, long userId)
    {
        var cacheData = await _cacheService.GetOrSetAsync(
            cacheKey: CacheKey.BAN_SW + userId,
            staleAfter: defaultStaleTime,
            action: async () => {
                SpamWatchResult spamwatchResult = new();

                var url = UrlConst.ANTISPAM_SPAMWATCH_API.AppendPathSegment(userId);

                var apiKey = await _apiKeyService.GetApiKeyAsync("INTERNAL", "SPAMWATCH");

                if (apiKey == null)
                    return spamwatchResult;

                var spamwatchDto = await url
                    .WithOAuthBearerToken(apiKey.ApiKey)
                    .AllowHttpStatus("404")
                    .GetJsonAsync<SpamwatchDto>();

                spamwatchResult.IsBanned = spamwatchDto.Code == 200;
                spamwatchResult.BanRecord = spamwatchDto;

                _logger.LogDebug("Spamwatch AntiSpam for {UserId}: {@Dto}", userId, spamwatchDto);

                return spamwatchResult;
            }
        );

        return cacheData;
    }
}