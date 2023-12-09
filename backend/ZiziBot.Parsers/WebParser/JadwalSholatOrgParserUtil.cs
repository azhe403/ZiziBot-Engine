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

    public static void GetMonthly()
    {
    }
}