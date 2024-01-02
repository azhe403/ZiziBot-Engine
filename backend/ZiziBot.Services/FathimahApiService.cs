using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using ZiziBot.Types.Vendor.FathimahApi.v2;

namespace ZiziBot.Services;

public class FathimahApiService
{
    private const string BaseUrl = "https://api.myquran.com/v2";

    private readonly ILogger<FathimahApiService> _logger;
    private readonly CacheService _cacheService;

    public FathimahApiService(ILogger<FathimahApiService> logger, CacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<CityResponse> GetAllCityAsync()
    {
        const string path = "sholat/kota/semua";

        _logger.LogInformation("Get City");

        var apis = await _cacheService.GetOrSetAsync(
            cacheKey: $"vendor/banghasan/{path}",
            expireAfter: "1d",
            staleAfter: "1h",
            action: async () => {
                var apis = await BaseUrl.AppendPathSegment(path).GetJsonAsync<CityResponse>();

                return apis;
            }
        );

        return apis;
    }

    public async Task<ShalatTimeResponse> GetShalatTime(long cityId)
    {
        return await GetShalatTime(DateTime.UtcNow.AddHours(Env.DEFAULT_TIMEZONE), cityId);
    }

    public async Task<ShalatTimeResponse> GetShalatTime(DateTime dateTime, long cityId)
    {
        var path = $"sholat/jadwal/{cityId}/{dateTime.Year}/{dateTime.Month}/{dateTime.Day}";

        _logger.LogInformation("Get Shalat time for ChatId: {CityId} with Date: {DateStr}", cityId, dateTime);

        var apis = await _cacheService.GetOrSetAsync(
            cacheKey: $"vendor/banghasan/{path}",
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