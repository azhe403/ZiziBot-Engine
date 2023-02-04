using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Services;

public class AntiSpamService
{
    private readonly ILogger<AntiSpamService> _logger;
    private readonly AntiSpamDbContext _antiSpamDbContext;

    public AntiSpamService(ILogger<AntiSpamService> logger, AntiSpamDbContext antiSpamDbContext)
    {
        _logger = logger;
        _antiSpamDbContext = antiSpamDbContext;
    }

    public async Task<bool> CheckSpamAsync(long chatId, long userId)
    {
        var taskCheckEss = CheckEssAsync(chatId, userId);
        var taskCheckCombotAntiSpam = CheckCombotAntiSpamAsync(chatId, userId);
        await Task.WhenAll(taskCheckEss, taskCheckCombotAntiSpam);

        var isEss = await taskCheckEss;
        var combotAntispamApiDto = await taskCheckCombotAntiSpam;

        return isEss || combotAntispamApiDto.Result != null;
    }

    public async Task<bool> CheckEssAsync(long chatId, long userId)
    {
        var globalBanEntities = await _antiSpamDbContext.GlobalBan
            .Where(entity => entity.UserId == userId && entity.Status == (int) EventStatus.Complete)
            .ToListAsync();

        return globalBanEntities.Any();
    }

    public async Task<CombotAntispamApiDto> CheckCombotAntiSpamAsync(long chatId, long userId)
    {
        var url = UrlConst.COMBOT_ANTISPAM_API.SetQueryParam("userId", userId).SetQueryParam("chatId", chatId);
        var antispamApiDto = await url.GetJsonAsync<CombotAntispamApiDto>();

        _logger.LogDebug("Combot AntiSpam for {UserId}: {@CombotAntispamApiDto}", userId, antispamApiDto);

        return antispamApiDto;
    }
}