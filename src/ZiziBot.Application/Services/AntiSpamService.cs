using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Services;

public class AntiSpamService
{
    private readonly ILogger<AntiSpamService> _logger;

    public AntiSpamService(ILogger<AntiSpamService> logger)
    {
        _logger = logger;
    }

    public async Task<CombotAntispamApiDto> CheckCombotAntiSpamAsync(long chatId, long userId)
    {
        var url = UrlConst.COMBOT_ANTISPAM_API.SetQueryParam("userId", userId).SetQueryParam("chatId", chatId);
        var antispamApiDto = await url.GetJsonAsync<CombotAntispamApiDto>();

        _logger.LogDebug("Combot AntiSpam for {UserId}: {@CombotAntispamApiDto}", userId, antispamApiDto);

        return antispamApiDto;
    }
}