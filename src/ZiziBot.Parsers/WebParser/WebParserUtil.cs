using System.Globalization;

namespace ZiziBot.Parsers.WebParser;

public static class WebParserUtil
{
    public static async Task<TrakteerParsedDto> ParseTrakteerWeb(this string url)
    {
        var trakteerParsedDto = new TrakteerParsedDto();
        var document = await url.OpenUrl();
        var container = document.QuerySelector("div.pr-container");
        var hasNode = container?.HasChildNodes;

        if (!hasNode.HasValue ||
            !hasNode.Value)
        {
            return trakteerParsedDto;
        }

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
        trakteerParsedDto.CendolsCount = cendolCount;
        trakteerParsedDto.Cendols = cendols;
        trakteerParsedDto.AdminFees = adminFees;
        trakteerParsedDto.Subtotal = subtotal;
        trakteerParsedDto.OrderDate = DateTime.ParseExact(orderDate ?? string.Empty, "dd MMMM yyyy, HH:mm", CultureInfo.InvariantCulture);
        trakteerParsedDto.PaymentMethod = paymentMethod;
        trakteerParsedDto.OrderId = orderId;
        trakteerParsedDto.RawText = innerText;

        return trakteerParsedDto;
    }
}