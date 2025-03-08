using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Application.Services;

public class JadwalSholatOrgSinkService(ILogger<JadwalSholatOrgSinkService> logger, DataFacade dataFacade)
{
    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
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

        var insertCities = cities?.Select(x => new JadwalSholatOrg_CityEntity() {
            CityId = x.CityId,
            CityCode = x.CityCode,
            CityName = x.CityName,
            Status = EventStatus.Complete,
            TransactionId = trxId
        });

        if (insertCities != null)
        {
            logger.LogDebug("Deleting old cities..");
            await dataFacade.MongoEf.JadwalSholatOrg_City.Where(entity => removeCityIds.Contains(entity.CityId)).ExecuteDeleteAsync();
            await dataFacade.MongoEf.SaveChangesAsync();

            logger.LogDebug("Inserting new cities..");
            dataFacade.MongoEf.JadwalSholatOrg_City.AddRange(insertCities);
            await dataFacade.MongoEf.SaveChangesAsync();
        }
    }

    public async Task FeedSchedule()
    {
        var cities = await dataFacade.MongoEf.JadwalSholatOrg_City.ToListAsync();

        foreach (var city in cities)
        {
            await FeedSchedule(city.CityId);
        }
    }


    public async Task<int> FeedSchedule(int cityId)
    {
        var trxId = Guid.NewGuid().ToString();
        var schedules = await JadwalSholatOrgParserUtil.FetchSchedules(cityId);

        var insertSchedules = schedules?.Select(x => new JadwalSholatOrg_ScheduleEntity() {
            CityId = cityId,
            Date = x.Date,
            Fajr = x.Fajr,
            Sunrise = x.Sunrise,
            Dhuhr = x.Dhuhr,
            Ashr = x.Ashr,
            Maghrib = x.Maghrib,
            Isha = x.Isha,
            Status = EventStatus.Complete,
            TransactionId = trxId
        }).ToList();

        if (insertSchedules.IsEmpty())
        {
            logger.LogInformation("No schedules found for city with ID {CityId}.", cityId);
            return default;
        }

        logger.LogDebug("Deleting old schedules for city {CityId}..", cityId);
        var jadwalSholatOrgSchedule = await dataFacade.MongoEf.JadwalSholatOrg_Schedule.Where(x => x.CityId == cityId).ToListAsync();
        dataFacade.MongoEf.RemoveRange(jadwalSholatOrgSchedule);
        await dataFacade.MongoEf.SaveChangesAsync();

        logger.LogDebug("Inserting new schedules for city {CityId}..", cityId);
        dataFacade.MongoEf.JadwalSholatOrg_Schedule.AddRange(insertSchedules);
        await dataFacade.MongoEf.SaveChangesAsync();

        return insertSchedules.Count;
    }
}