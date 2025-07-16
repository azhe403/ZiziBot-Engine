using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using ZiziBot.Common.Types;
using ZiziBot.Common.Vendor.FathimahApi.v2;
using ZiziBot.Database.Service;

namespace ZiziBot.Services.Rest;

public sealed class FathimahRestService(
    ILogger<FathimahRestService> logger,
    CacheService cacheService
)
{
    public async Task<CityResponse> GetAllCityAsync()
    {
        const string path = "sholat/kota/semua";

        logger.LogInformation("Get City");

        var apis = await cacheService.GetOrSetAsync(new Cache<CityResponse>(){
            CacheKey =  $"vendor/bang-hasan/{path}",
            ExpireAfter =  "1d",
            StaleAfter = "1h",
            EvictAfter = true,
            Action = async () => {
                var apis = await UrlConst.FATHIMAH_API
                    .AppendPathSegment(path)
                    .GetJsonAsync<CityResponse>();

                return apis;
            }
        });

        return apis;
    }

    public async Task<KeyValuePair<string, string>?> GetCurrentShalatTime(long cityId)
    {
        var currentTime = DateTime.UtcNow.AddHours(Env.DEFAULT_TIMEZONE).ToString("HH:mm");
        var shalatTime = await GetShalatTime(DateTime.Now, cityId);
        var currentShalat = shalatTime.Schedule?.ShalatDict.FirstOrDefault(pair => pair.Value == currentTime);

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

        var apis = await cacheService.GetOrSetAsync(new Cache<ShalatTimeResponse>(){
            CacheKey = $"vendor/bang-hasan/{path}",
            ExpireAfter = "1d",
            EvictBefore = evictBefore,
            StaleAfter = "1h",
            Action = async () => {
                var apis = await UrlConst.FATHIMAH_API
                    .AppendPathSegment(path)
                    .GetJsonAsync<ShalatTimeResponse>();

                return apis;
            }}
        );

        logger.LogInformation("Shalat time for ChatId: {CityId} with Date: {DateStr} is: {ShalatTime}", cityId, dateTime, apis.Schedule?.Daerah);

        return apis;
    }
}