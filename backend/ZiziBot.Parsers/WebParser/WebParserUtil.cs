using System.Globalization;
using Flurl;
using Flurl.Http;

namespace ZiziBot.Parsers.WebParser;

public static class WebParserUtil
{
    public static async Task<TrakteerParsedDto> ParseTrakteerWeb(this string url)
    {
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
        var hasNode = container?.HasChildNodes;

        if (container == null)
        {
            Log.Information("Not found container for url: {Url}", url);
            return trakteerParsedDto;
        }

        Log.Debug("Found container: {Container} in Url: {Url}", container?.ClassName, url);

        var cendolCount = document.QuerySelector("#wrapper > div > div > div.pr-detail > div.pr-detail__subtotal > div.subtotal-left > div.subtotal-left__unit > span")
            ?.TextContent;
        var cendols = document.QuerySelector("#wrapper > div > div > div.pr-detail > div.pr-detail__subtotal > div.subtotal-left > div:nth-child(2) > span:nth-child(2)")
            ?.TextContent;
        var adminFees = document
            .QuerySelector("#wrapper > div > div > div.pr-detail > div.pr-detail__subtotal > div.subtotal-left > div:nth-child(4) > span:nth-child(2)")?.TextContent;
        var subtotal = document.QuerySelector("#wrapper > div > div > div.pr-detail > div.pr-detail__subtotal > div.subtotal-right")?.TextContent;
        var orderDate = document.QuerySelector("#wrapper > div > div > div.pr-detail > div.pr-detail__wrapper > div:nth-child(1) > div:nth-child(1) > div:nth-child(2)")
            ?.TextContent.Replace("WIB", string.Empty).Trim();
        var paymentMethod = document
            .QuerySelector("#wrapper > div > div > div.pr-detail > div.pr-detail__wrapper > div:nth-child(1) > div:nth-child(2) > div:nth-child(2)")?.TextContent;
        var orderId = document.QuerySelector("#wrapper > div > div > div.pr-detail > div.pr-detail__wrapper > div:nth-child(2) > div:nth-child(2)")?.TextContent;

        var mainNode = container?.ChildNodes
            .Skip(1)
            .SkipLast(1);

        var innerText = mainNode.Select(x => x.TextContent)
            .Aggregate((s1, s2) => $"{s1}\n{s2}");

        trakteerParsedDto.IsValid = innerText?.Contains("Pembayaran Berhasil") ?? false;
        trakteerParsedDto.PaymentUrl = url;
        trakteerParsedDto.Cendols = cendolCount;
        trakteerParsedDto.CendolCount = int.Parse(cendolCount.Replace("Cendol", string.Empty).Trim(), CultureInfo.InvariantCulture);
        trakteerParsedDto.AdminFees = adminFees;
        trakteerParsedDto.Subtotal = subtotal;
        trakteerParsedDto.OrderDate = DateTime.ParseExact(orderDate ?? string.Empty, "dd MMMM yyyy, HH:mm", CultureInfo.InvariantCulture);
        trakteerParsedDto.PaymentMethod = paymentMethod;
        trakteerParsedDto.OrderId = orderId;
        trakteerParsedDto.RawText = innerText;

        Log.Information("Parsed trakteer url: {Url}", url);

        return trakteerParsedDto;
    }

    public static async Task<TrakteerApiDto> GetTrakteerApi(this string url)
    {
        var data = await UrlConst.API_TRAKTEER_PARSER.SetQueryParam("url", url)
            .GetJsonAsync<TrakteerApiDto>();

        data.IsValid = true;
        data.PaymentUrl = url;

        return data;
    }
}