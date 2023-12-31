using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Services;

public class JadwalSholatOrgSinkService
{
    private readonly ILogger<JadwalSholatOrgSinkService> _logger;
    private readonly MongoDbContextBase _mongoDbContext;

    public JadwalSholatOrgSinkService(ILogger<JadwalSholatOrgSinkService> logger, MongoDbContextBase mongoDbContext)
    {
        _logger = logger;
        _mongoDbContext = mongoDbContext;
    }

    public async Task FeedAll()
    {
        await FeedCity();
        await FeedSchedule();
    }

    public async Task FeedCity()
    {
        var trxId = Guid.NewGuid().ToString();
        var cities = await JadwalSholatOrgParserUtil.GetCities();
        var removeCityIds = cities.Select(x => x.CityId);

        var insertCities = cities?.Select(x => new JadwalSholatOrg_CityEntity()
        {
            CityId = x.CityId,
            CityCode = x.CityCode,
            CityName = x.CityName,
            Status = (int)EventStatus.Complete,
            TransactionId = trxId
        });

        if (insertCities != null)
        {
            _logger.LogDebug("Deleting old cities..");
            _mongoDbContext.JadwalSholatOrg_City.RemoveRange(entity => removeCityIds.Contains(entity.CityId));
            await _mongoDbContext.SaveChangesAsync();

            _logger.LogDebug("Inserting new cities..");
            _mongoDbContext.JadwalSholatOrg_City.AddRange(insertCities);
            await _mongoDbContext.SaveChangesAsync();
        }
    }

    public async Task FeedSchedule()
    {
        var cities = await _mongoDbContext.JadwalSholatOrg_City.ToListAsync();

        foreach (var city in cities)
        {
            await FeedSchedule(city.CityId);
        }
    }


    public async Task FeedSchedule(int cityId)
    {
        var trxId = Guid.NewGuid().ToString();
        var schedules = await JadwalSholatOrgParserUtil.FetchSchedules(cityId);

        var insertSchedules = schedules?.Select(x => new JadwalSholatOrg_ScheduleEntity()
        {
            CityId = cityId,
            Date = x.Date,
            Fajr = x.Fajr,
            Sunrise = x.Sunrise,
            Dhuhr = x.Dhuhr,
            Ashr = x.Ashr,
            Maghrib = x.Maghrib,
            Isha = x.Isha,
            Status = (int)EventStatus.Complete,
            TransactionId = trxId
        });

        if (insertSchedules != null)
        {
            _logger.LogDebug("Deleting old schedules for city {cityId}..", cityId);
            _mongoDbContext.JadwalSholatOrg_Schedule.RemoveRange(x => x.CityId == cityId);
            await _mongoDbContext.SaveChangesAsync();

            _logger.LogDebug("Inserting new schedules for city {cityId}..", cityId);
            _mongoDbContext.JadwalSholatOrg_Schedule.AddRange(insertSchedules);
            await _mongoDbContext.SaveChangesAsync();
        }
    }
}