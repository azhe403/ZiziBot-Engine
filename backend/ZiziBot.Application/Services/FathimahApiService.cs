using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Services;

public class FathimahApiService
{
    private const string BaseUrl = "https://api.banghasan.com";
    private readonly ILogger<FathimahApiService> _logger;
    private readonly CacheService _cacheService;

    public FathimahApiService(ILogger<FathimahApiService> logger, CacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<CityResponse> GetAllCityAsync()
    {
        const string path = "sholat/format/json/kota";

        _logger.LogInformation("Get City");

        var apis = await _cacheService.GetOrSetAsync(
            cacheKey: $"vendor/{BaseUrl}/{path}".ForCacheKey(),
            expireAfter: "1d",
            staleAfter: "1h",
            action: async () => {
                var apis = await BaseUrl.AppendPathSegment(path).GetJsonAsync<CityResponse>();

                return apis;
            }
        );

        return apis;
    }

    public async Task<ShalatTimeResponse> GetShalatTime(DateOnly dateTime, long cityId)
    {
        var dateStr = dateTime.ToString("yyyy-MM-dd");
        var  path = $"sholat/format/json/jadwal/kota/{cityId}/tanggal/{dateStr}";

        _logger.LogInformation("Get Shalat time for ChatId: {CityId} with Date: {DateStr}", cityId, dateStr);

        var apis = await _cacheService.GetOrSetAsync(
            cacheKey: $"vendor/{BaseUrl}/{path}".ForCacheKey(),
            expireAfter: "1d",
            staleAfter: "1h",
            action: async () => {
                var apis = await BaseUrl
                    .AppendPathSegment(path)
                    .GetJsonAsync<ShalatTimeResponse>();

                return apis;
            }
        );

        return apis;
    }
}