using AngleSharp.Html.Dom;
using ZiziBot.Types.Vendor.JadwalSholatOrg;

namespace ZiziBot.Parsers.WebParser;

public static class JadwalSholatOrgParserUtil
{
    private const string WebUrl = "https://www.jadwalsholat.org/adzan/monthly.php";

    public static async Task<IEnumerable<City>?> GetCities()
    {
        var document = await WebUrl.OpenUrl();

        var cities = document?.QuerySelector<IHtmlSelectElement>("select[class=town-select]")?.Options.Select(x => new City() {
            CityId = x.Value.Convert<int>(),
            CityCode = x.Text.ToLower().RegexReplace("[^a-zA-Z]", ""),
            CityName = x.Text
        });

        return cities;
    }

    public static async Task<IEnumerable<IEnumerable<ShalatTime>?>> FetchSchedulesFullYear(int cityId)
    {
        var year = DateTime.UtcNow.Year;
        var months = Enumerable.Range(1, 12);

        var monthTask = await months.Select(x => FetchSchedules(cityId, x, year)).WhenAll(1);

        return monthTask;
    }

    public static async Task<IEnumerable<ShalatTime>?> FetchSchedules(int cityId)
    {
        var year = DateTime.UtcNow.Year;
        var month = DateTime.UtcNow.Month;

        var tableRows = await FetchSchedules(cityId, month, year);

        return tableRows;
    }

    public static async Task<IEnumerable<ShalatTime>?> FetchSchedules(int cityId, int month)
    {
        var year = DateTime.UtcNow.Year;

        return await FetchSchedules(cityId, month, year);
    }


    public static async Task<IEnumerable<ShalatTime>?> FetchSchedules(int cityId, int month, int year)
    {
        var webUrl = $"{WebUrl}?id={cityId}&m={month}&y={year}";
        var document = await webUrl.OpenUrl();

        var tableRows = document?.QuerySelectorAll<IHtmlTableRowElement>("table[class=table_adzan] > tbody > tr");
        var times = tableRows?.Skip(2)
            .Select(x => x.Cells.Select(y => y.TextContent.RegexMatchIf("[0-9][0-9]:[0-9][0-9]+", y.TextContent.Contains(':')))).ToList();
        var header = times?.First();

        var shalatTimes = times?.Skip(1)
            .Select(row => header?.Zip(row, (h, t) => new { h, t }).ToDictionary(ht => ht.h, ht => ht.t))
            .SkipLast(13)
            .Select(x => {
                var date = x.GetValueOrDefault("Tanggal");

                var time = new ShalatTime {
                    Date = new DateOnly(year, month, date.Convert<int>()),
                    Fajr = TimeOnly.Parse(x.GetValueOrDefault("Shubuh")),
                    Sunrise = TimeOnly.Parse(x.GetValueOrDefault("Terbit")),
                    Dhuhr = TimeOnly.Parse(x.GetValueOrDefault("Dzuhur")),
                    Ashr = TimeOnly.Parse(x.GetValueOrDefault("Ashr")),
                    Maghrib = TimeOnly.Parse(x.GetValueOrDefault("Maghrib")),
                    Isha = TimeOnly.Parse(x.GetValueOrDefault("Isya"))
                };

                return time;
            });

        return shalatTimes;
    }
}