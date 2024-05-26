using System.Net;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using ZiziBot.Interfaces;

namespace ZiziBot.Application.Services;

public class AntiSpamService(
    ILogger<AntiSpamService> logger,
    ICacheService cacheService,
    MongoDbContextBase mongoDbContext,
    ApiKeyService apiKeyService
)
{
    private const string DefaultStaleTime = "10m";

    public async Task<AntiSpamDto> CheckSpamAsync(long chatId, long userId)
    {
        var antispamDto = new AntiSpamDto();

        if (userId == 0)
            return antispamDto;

        var taskCheckEss = CheckEssAsync(chatId, userId);
        var taskCheckCombotAntiSpam = CheckCombotAntiSpamAsync(chatId, userId);
        var taskCheckSpamWatch = CheckSpamWatchAntiSpamAsync(chatId, userId);
        await Task.WhenAll(taskCheckEss, taskCheckCombotAntiSpam, taskCheckSpamWatch);

        antispamDto.IsBanEss = (await taskCheckEss).IsBanEss;
        antispamDto.IsBanCasFed = (await taskCheckCombotAntiSpam).IsBanCasFed;
        antispamDto.IsBanSwFed = (await taskCheckSpamWatch).IsBanSwFed;

        antispamDto.CasRecord = (await taskCheckCombotAntiSpam).CasRecord;
        antispamDto.SpamWatchRecord = (await taskCheckSpamWatch).SpamWatchRecord;

        antispamDto.IsBanAny = antispamDto.IsBanEss || antispamDto.IsBanCasFed || antispamDto.IsBanSwFed;

        return antispamDto;
    }

    private async Task<AntiSpamDto> CheckEssAsync(long chatId, long userId)
    {
        var cacheData = await cacheService.GetOrSetAsync(
            cacheKey: CacheKey.USER_BAN_ESS + userId,
            staleAfter: DefaultStaleTime,
            action: async () => {
                var antiSpamDto = new AntiSpamDto();
                var globalBanEntities = await mongoDbContext.GlobalBan
                    .Where(entity => entity.UserId == userId && entity.Status == (int)EventStatus.Complete)
                    .ToListAsync();

                antiSpamDto.IsBanEss = globalBanEntities.Count != 0;
                return antiSpamDto;
            }
        );

        return cacheData;
    }

    private async Task<AntiSpamDto> CheckCombotAntiSpamAsync(long chatId, long userId)
    {
        var cacheData = await cacheService.GetOrSetAsync(
            cacheKey: CacheKey.USER_BAN_CAS + userId,
            staleAfter: DefaultStaleTime,
            action: async () => {
                var antiSpamDto = new AntiSpamDto();
                try
                {
                    var url = UrlConst.ANTISPAM_COMBOT_API.SetQueryParam("userId", userId);
                    var antispamApiDto = await url.AllowAnyHttpStatus().GetJsonAsync<CombotAntispamApiDto>();

                    logger.LogDebug("Combot AntiSpam for {UserId}: {@Dto}", userId, antispamApiDto);
                    antiSpamDto.IsBanCasFed = antispamApiDto.IsBanned;
                    antiSpamDto.CasRecord = antispamApiDto.Result;
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Fail to check Combot AntiSpam for {UserId}", userId);
                }

                return antiSpamDto;
            }
        );

        return cacheData;
    }

    private async Task<AntiSpamDto> CheckSpamWatchAntiSpamAsync(long chatId, long userId)
    {
        var cacheData = await cacheService.GetOrSetAsync(
            cacheKey: CacheKey.USER_BAN_SW + userId,
            staleAfter: DefaultStaleTime,
            action: async () => {
                SpamWatchResult spamwatchResult = new();
                var antiSpamDto = new AntiSpamDto();

                try
                {
                    var url = UrlConst.ANTISPAM_SPAMWATCH_API.AppendPathSegment(userId);
                    var apiKey = await apiKeyService.GetApiKeyAsync(ApiKeyCategory.Internal, ApiKeyVendor.SpamWatch);

                    if (apiKey.IsNotNullOrEmpty())
                        return antiSpamDto;

                    var spamwatchDto = await url.AllowAnyHttpStatus()
                        .WithOAuthBearerToken(apiKey)
                        .GetJsonAsync<SpamwatchDto>();

                    spamwatchResult.IsBanned = spamwatchDto.Code == (int)HttpStatusCode.OK;
                    spamwatchResult.BanRecord = spamwatchDto;
                    antiSpamDto.SpamWatchRecord = spamwatchDto;

                    logger.LogDebug("Spamwatch AntiSpam for {UserId}: {@Dto}", userId, spamwatchDto);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Fail to check Spamwatch AntiSpam for {UserId}", userId);
                }

                return antiSpamDto;
            }
        );

        return cacheData;
    }
}