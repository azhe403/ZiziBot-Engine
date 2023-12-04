using System.Globalization;
using CloudflareSolverRe;
using Flurl;
using Flurl.Http;
using PickAll;

namespace ZiziBot.Parsers.WebParser;

public static class WebParserUtil
{
    public static async Task<TrakteerParsedDto> ParseTrakteerWeb(this string url)
    {
        url = url.GetTrakteerUrl();
        var trakteerParsedDto = new TrakteerParsedDto();
        Log.Information("Parsing trakteer url: {Url}", url);
        var document = await url.OpenUrl(new ClearanceHandler()
        {
            ClearanceDelay = 3000
        });

        if (document == null)
        {
            Log.Error("Cannot load url: {Url}", url);
            return trakteerParsedDto;
        }

        Log.Debug("Web title of Url: {Url} => {Title}", url, document.Title);

        var container = document.QuerySelector("div.pr-container");

        if (container == null)
        {
            Log.Information("Not found container for url: {Url}", url);
            return trakteerParsedDto;
        }

        Log.Debug("Found container: {Container} in Url: {Url}", container?.ClassName, url);

        var cendolCount = document.QuerySelector(".subtotal-left__unit > span:nth-child(2)")?.TextContent;
        var adminFees = document.QuerySelector("div.subtotal-left__others:nth-child(4) > span:nth-child(2)")?.TextContent;
        var subtotal = document.QuerySelector("#wrapper > div > div > div.pr-detail > div.pr-detail__subtotal > div.subtotal-right")?.TextContent;
        var orderDate = document.QuerySelector("div.pr-detail__item:nth-child(1) > div:nth-child(2)")?.TextContent.Replace("WIB", string.Empty).Trim();
        var paymentMethod = document.QuerySelector(".pr-detail__wrapper > div:nth-child(1) > div:nth-child(2) > div:nth-child(2)")?.TextContent;
        var orderId = document.QuerySelector(".pr-detail__wrapper > div:nth-child(2) > div:nth-child(2)")?.TextContent;

        var mainNode = container?.ChildNodes
            .Skip(1)
            .SkipLast(1);

        var innerText = mainNode?.Select(x => x.TextContent)
            .Aggregate((s1, s2) => $"{s1}\n{s2}");

        trakteerParsedDto.IsValid = innerText?.Contains("Pembayaran Berhasil") ?? false;
        trakteerParsedDto.PaymentUrl = url;
        trakteerParsedDto.Cendols = cendolCount;
        trakteerParsedDto.CendolCount = cendolCount.Replace("Cendol", string.Empty).Trim().Convert<int>();
        trakteerParsedDto.AdminFees = adminFees.Replace("Rp", "").Trim().Convert<int>();
        trakteerParsedDto.Subtotal = subtotal.Replace("Rp", "").Trim().Convert<int>();
        trakteerParsedDto.OrderDate = DateTime.ParseExact(orderDate ?? string.Empty, "dd MMMM yyyy, HH:mm", CultureInfo.InvariantCulture);
        trakteerParsedDto.PaymentMethod = paymentMethod;
        trakteerParsedDto.OrderId = orderId;
        trakteerParsedDto.RawText = innerText;

        Log.Information("Parsed trakteer url: {Url}", url);

        return trakteerParsedDto;
    }

    public static async Task<TrakteerParsedDto> ParseSaweriaWeb(this string url)
    {
        url = url.GetSaweriaUrl();
        var trakteerParsedDto = new TrakteerParsedDto();
        Log.Information("Parsing trakteer url: {Url}", url);
        var document = await url.OpenUrl();
        if (document == null)
        {
            Log.Error("Cannot load url: {Url}", url);
            return trakteerParsedDto;
        }

        Log.Debug("Web title of Url: {Url} => {Title}", url, document.Title);

        var container = document.QuerySelector("div.pr-container");

        if (container == null)
        {
            Log.Information("Not found container for url: {Url}", url);
            return trakteerParsedDto;
        }

        Log.Debug("Found container: {Container} in Url: {Url}", container?.ClassName, url);

        return default;
    }

    public static async Task<TrakteerApiDto> GetTrakteerApi(this string url)
    {
        if (!url.StartsWith("https://trakteer.id/payment-status"))
        {
            url = Url.Combine("https://trakteer.id/payment-status", url);
        }

        var data = await UrlConst.API_TRAKTEER_PARSER.SetQueryParam("url", url, true)
            .GetJsonAsync<TrakteerApiDto>();

        data.IsValid = data.OrderId != null;
        data.PaymentUrl = url;

        return data;
    }

    public static async Task<SaweriaParsedDto> GetSaweriaApi(this string url)
    {
        if (!url.StartsWith("https://saweria.co/receipt"))
        {
            url = Url.Combine("https://saweria.co/receipt", url);
        }

        var data = await UrlConst.API_SAWERIA_PARSER.SetQueryParam("oid", url, true)
            .GetJsonAsync<SaweriaParsedDto>();

        data.IsValid = data.OrderId != null;
        data.CendolCount = data.Total / 5000;
        data.PaymentUrl = url;

        return data;
    }

    public static async Task<IEnumerable<WebSearch>> WebSearchText(string search)
    {
        var ctx = await new SearchContext()
            .WithEvents()
            .With<Google>()
            .With<Uniqueness>()
            .SearchAsync(search);

        return ctx.Select(x => new WebSearch()
        {
            Title = x.Description,
            Url = x.Url.UrlDecode()
        });
    }

    public static Task<string> HtmlForTelegram(this string htmlString)
    {
        var htmlCleaned = htmlString.Replace("<br>", "").Replace("</br>", "")
            .Replace("<p>", "\n").Replace("</p>", "")
            .Replace("<ul>", "").Replace("</ul>", "")
            .Replace("<li>", "- ").Replace("</li>", "")
            .RegexReplace("<h.|<.h.>", "");

        return Task.FromResult(htmlCleaned);
    }
}