using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Services;

public class AntiSpamService
{
    private readonly ILogger<AntiSpamService> _logger;
    private readonly AntiSpamDbContext _antiSpamDbContext;
    private readonly CacheService _cacheService;

    public AntiSpamService(ILogger<AntiSpamService> logger, AntiSpamDbContext antiSpamDbContext, CacheService cacheService)
    {
        _logger = logger;
        _antiSpamDbContext = antiSpamDbContext;
        _cacheService = cacheService;
    }

    public async Task<AntiSpamDto> CheckSpamAsync(long chatId, long userId)
    {
        var antispamDto = new AntiSpamDto();

        if (userId == 0)
            return antispamDto;

        var taskCheckEss = CheckEssAsync(chatId, userId);
        var taskCheckCombotAntiSpam = CheckCombotAntiSpamAsync(chatId, userId);
        await Task.WhenAll(taskCheckEss, taskCheckCombotAntiSpam);

        antispamDto.IsBanEss = await taskCheckEss;
        antispamDto.IsBanCasFed = (await taskCheckCombotAntiSpam).Result != null;
        antispamDto.CasRecord = (await taskCheckCombotAntiSpam).Result;
        antispamDto.IsBanAny = antispamDto.IsBanEss || antispamDto.IsBanCasFed;

        return antispamDto;
    }

    private async Task<bool> CheckEssAsync(long chatId, long userId)
    {
        var cacheData = await _cacheService.GetOrSetAsync(
            cacheKey: "ban_ess_" + userId,
            action: async () => {
                var globalBanEntities = await _antiSpamDbContext.GlobalBan
                    .Where(entity => entity.UserId == userId && entity.Status == (int) EventStatus.Complete)
                    .ToListAsync();

                return globalBanEntities.Any();
            }
        );

        return cacheData;
    }

    private async Task<CombotAntispamApiDto> CheckCombotAntiSpamAsync(long chatId, long userId)
    {
        var cacheData = await _cacheService.GetOrSetAsync(
            cacheKey: "ban_combot_" + userId,
            action: async () => {
                var url = UrlConst.COMBOT_ANTISPAM_API.SetQueryParam("userId", userId);
                var antispamApiDto = await url.GetJsonAsync<CombotAntispamApiDto>();

                _logger.LogDebug("Combot AntiSpam for {UserId}: {@Dto}", userId, antispamApiDto);

                return antispamApiDto;
            }
        );

        return cacheData;
    }
}