using AngleSharp.Html.Dom;
using ZiziBot.Types.Vendor.JadwalSholatOrg;

namespace ZiziBot.Parsers.WebParser;

public static class JadwalSholatOrgParserUtil
{
    private const string WebUrl = "https://www.jadwalsholat.org/adzan/monthly.php";

    public static async Task<IEnumerable<City>?> GetCities()
    {
        var document = await WebUrl.OpenUrl();

        var cities = document?.QuerySelectorAll("option")
            .Cast<IHtmlOptionElement>()
            .Select(x => new City()
            {
                CityId = x.Value.Convert<int>(),
                CityCode = x.Text.ToLower().RegexReplace("[^a-zA-Z]", ""),
                CityName = x.Text
            });

        return cities;
    }

    public static async Task<IEnumerable<List<ShalatTime>?>> FetchSchedules(int cityId)
    {
        var year = DateTime.UtcNow.Year;
        var months = Enumerable.Range(1, 12);

        var monthTask = await months.Select(x => FetchSchedules(cityId, year, x)).WhenAll(1);

        return monthTask;
    }

    public static async Task<List<ShalatTime>?> FetchSchedules(int cityId, int year)
    {
        var month = DateTime.UtcNow.Month;

        return await FetchSchedules(cityId, year, month);
    }


    public static async Task<List<ShalatTime>?> FetchSchedules(int cityId, int year, int month)
    {
        var webUrl = $"{WebUrl}?id={cityId}&m={month}&y={year}";
        var document = await webUrl.OpenUrl();

        var tableRows = document?.QuerySelectorAll<IHtmlTableRowElement>("table[class=table_adzan] > tbody > tr");
        var times = tableRows?.Skip(2).Select(x => x.Cells.Select(y => y.TextContent)).ToList();
        var header = times?.First();

        var shalatTimes = times?.Skip(1)
            .Select(row => header?.Zip(row, (h, t) => new { h, t }).ToDictionary(ht => ht.h, ht => ht.t))
            .SkipLast(13)
            .Select(x =>
            {
                var date = x.GetValueOrDefault("Tanggal");

                var time = new ShalatTime
                {
                    Date = new DateOnly(year, month, date.Convert<int>()),
                    Fajr = TimeOnly.Parse(x.GetValueOrDefault("Shubuh")),
                    Sunrise = TimeOnly.Parse(x.GetValueOrDefault("Terbit")),
                    Dhuhr = TimeOnly.Parse(x.GetValueOrDefault("Dzuhur")),
                    Ashr = TimeOnly.Parse(x.GetValueOrDefault("Ashr")),
                    Maghrib = TimeOnly.Parse(x.GetValueOrDefault("Maghrib")),
                    Isha = TimeOnly.Parse(x.GetValueOrDefault("Isya"))
                };

                return time;
            })
            .ToList();

        return shalatTimes;
    }
}