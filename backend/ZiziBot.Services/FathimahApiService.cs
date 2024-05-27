using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using ZiziBot.Types.Vendor.FathimahApi.v2;

namespace ZiziBot.Services;

public class FathimahApiService(
    ILogger<FathimahApiService> logger,
    ICacheService cacheService
)
{
    private const string BaseUrl = "https://api.myquran.com/v2";

    public async Task<CityResponse> GetAllCityAsync()
    {
        const string path = "sholat/kota/semua";

        logger.LogInformation("Get City");

        var apis = await cacheService.GetOrSetAsync(
            cacheKey: $"vendor/banghasan/{path}",
            expireAfter: "1d",
            staleAfter: "1h",
            evictAfter: true,
            action: async () => {
                var apis = await BaseUrl.AppendPathSegment(path).GetJsonAsync<CityResponse>();

                return apis;
            }
        );

        return apis;
    }

    public async Task<KeyValuePair<string, string>?> GetCurrentShalatTime(long cityId)
    {
        var currentTime = DateTime.UtcNow.AddHours(Env.DEFAULT_TIMEZONE).ToString("HH:mm");
        var shalatTime = await GetShalatTime(DateTime.Now, cityId);
        var currentShalat = shalatTime.Schedule?.ShalatDict?.FirstOrDefault(pair => pair.Value == currentTime);

        return currentShalat;
    }

    public async Task<ShalatTimeResponse> GetShalatTime(long cityId, bool evictBefore = false)
    {
        return await GetShalatTime(DateTime.UtcNow.AddHours(Env.DEFAULT_TIMEZONE), cityId, evictBefore);
    }

    public async Task<ShalatTimeResponse> GetShalatTime(DateTime dateTime, long cityId, bool evictBefore = false)
    {
        var date = dateTime.ToString("yyyy-MM-dd");
        var path = $"sholat/jadwal/{cityId}/{date}";

        logger.LogInformation("Get Shalat time for ChatId: {CityId} with Date: {DateStr}", cityId, dateTime);

        var apis = await cacheService.GetOrSetAsync(
            cacheKey: $"vendor/banghasan/{path}",
            expireAfter: "1d",
            evictBefore: evictBefore,
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