using System.Globalization;
using AngleSharp.Html.Dom;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using ZiziBot.Contracts.Dtos;
using ZiziBot.Contracts.Enums;

namespace ZiziBot.Services;

public class MirrorPaymentService(
    ILogger<MirrorPaymentService> logger,
    AppSettingRepository appSettingRepository
)
{
    public async Task<ParsedDonationDto> ParseDonation(string orderId)
    {
        logger.LogInformation("Checking donation for OrderId: {OrderId}", orderId);
        var parsedDonationDto = await ParseTrakteerWeb(orderId);

        if (parsedDonationDto.IsValid)
        {
            return parsedDonationDto;
        }

        logger.LogDebug("Continue to Saweria for OrderId: {OrderId}", orderId);
        parsedDonationDto = await ParseSaweriaWeb(orderId);
        if (parsedDonationDto.IsValid)
        {
            return parsedDonationDto;
        }

        logger.LogWarning("Unknown donation source for OrderId: {OrderId}", orderId);
        parsedDonationDto.Source = DonationSource.Unknown;

        return parsedDonationDto;
    }

    public async Task<ParsedDonationDto> ParseTrakteerWeb(string url)
    {
        url = url.GetTrakteerUrl();
        var parsedDonationDto = new ParsedDonationDto();
        Log.Information("Parsing trakteer url: {Url}", url);
        var document = await url.OpenUrl(new() {
            ClearanceDelay = 3000
        });

        if (document == null)
        {
            Log.Error("Cannot load url: {Url}", url);
            return parsedDonationDto;
        }

        Log.Debug("Web title of Url: {Url} => {Title}", url, document.Title);

        var container = document.QuerySelector("div.pr-container");

        if (container == null)
        {
            Log.Information("Not found container for url: {Url}", url);
            return parsedDonationDto;
        }

        Log.Debug("Found container: {Container} in Url: {Url}", container?.ClassName, url);

        var cendolCount = document.QuerySelector("span.text-no-wrap:nth-child(4)")?.TextContent ?? "0";
        var adminFees = document.QuerySelector("div.subtotal-left__others:nth-child(2) > span:nth-child(2)")?.TextContent ?? "0";
        var subtotal = document.QuerySelector("#wrapper > div > div > div.pr-detail > div.pr-detail__subtotal > div.subtotal-right")?.TextContent ?? "0";
        var orderDate = document.QuerySelector("div.pr-detail__item:nth-child(1) > div:nth-child(2)")?.TextContent.Replace("WIB", string.Empty).Trim();
        var paymentMethod = document.QuerySelector(".pr-detail__wrapper > div:nth-child(1) > div:nth-child(2) > div:nth-child(2)")?.TextContent;
        var orderId = document.QuerySelector(".pr-detail__wrapper > div:nth-child(2) > div:nth-child(2)")?.TextContent;

        var mainNode = container?.ChildNodes.Skip(1).SkipLast(1);

        var innerText = mainNode?.Select(x => x.TextContent).Aggregate((s1, s2) => $"{s1}\n{s2}");

        parsedDonationDto.IsValid = innerText?.Contains("Pembayaran Berhasil") ?? false;
        parsedDonationDto.OrderId = orderId;
        parsedDonationDto.PaymentUrl = url;

        if (orderId.IsNullOrEmpty())
            return parsedDonationDto;

        parsedDonationDto.Source = DonationSource.Trakteer;
        parsedDonationDto.Cendols = cendolCount;
        parsedDonationDto.AdminFees = adminFees.Replace("Rp", "").Trim().Convert<int>();
        parsedDonationDto.Subtotal = subtotal.Replace("Rp", "").Trim().Convert<int>();
        parsedDonationDto.OrderDate = DateTime.ParseExact(orderDate ?? string.Empty, "dd MMMM yyyy, HH:mm", CultureInfo.InvariantCulture);

        parsedDonationDto.PaymentMethod = paymentMethod;
        parsedDonationDto.RawText = innerText;

        Log.Information("Parsed trakteer url: {Url}", url);

        return parsedDonationDto;
    }

    public async Task<ParsedDonationDto> ParseSaweriaWeb(string url)
    {
        url = url.GetSaweriaUrl();
        var parsedDonationDto = new ParsedDonationDto();
        Log.Information("Parsing saweria url: {Url}", url);
        var document = await url.OpenUrl();
        if (document == null)
        {
            Log.Error("Cannot load url: {Url}", url);
            return parsedDonationDto;
        }

        Log.Debug("Web title of Url: {Url} => {Title}", url, document.Title);

        var amount = document.QuerySelector<IHtmlHeadingElement>(".css-11qagvt")?.TextContent ?? "0";
        var status = document.QuerySelector<IHtmlHeadingElement>(".css-14dtuui")?.TextContent;
        var date = document.QuerySelectorAll<IHtmlInputElement>(".css-1a8pd4a")?.ElementAtOrDefault(0)?.Value;
        var time = document.QuerySelectorAll<IHtmlInputElement>(".css-1a8pd4a")?.ElementAtOrDefault(1)?.Value;
        var orderId = document.QuerySelectorAll<IHtmlInputElement>(".css-1a8pd4a")?.ElementAtOrDefault(2)?.Value;


        parsedDonationDto.OrderId = orderId;
        parsedDonationDto.PaymentUrl = url;

        if (orderId.IsNullOrEmpty())
            return parsedDonationDto;

        parsedDonationDto.Subtotal = amount.Replace("Rp", "").Trim().Convert<int>();
        parsedDonationDto.OrderDate = DateTime.ParseExact($"{date} {time}", "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
        parsedDonationDto.IsValid = true;
        parsedDonationDto.Source = DonationSource.Saweria;

        return parsedDonationDto;
    }

    public async Task<TrakteerApiDto> GetTrakteerApi(string url)
    {
        if (!url.StartsWith("https://trakteer.id/payment-status"))
        {
            url = Url.Combine("https://trakteer.id/payment-status", url);
        }

        var data = await GetTrakteerApi().SetQueryParam("url", url, true).GetJsonAsync<TrakteerApiDto>();

        data.IsValid = data.OrderId != null;
        data.PaymentUrl = url;

        return data;
    }

    public async Task<SaweriaParsedDto> GetSaweriaApi(string url)
    {
        if (!url.StartsWith("https://saweria.co/receipt"))
        {
            url = Url.Combine("https://saweria.co/receipt", url);
        }

        var data = await GetSaweriaApi().SetQueryParam("oid", url, true).GetJsonAsync<SaweriaParsedDto>();

        data.IsValid = data.OrderId != null;
        data.CendolCount = data.Total / 5000;
        data.PaymentUrl = url;

        return data;
    }

    private string GetTrakteerApi()
    {
        var config = appSettingRepository.GetRequiredConfigSection<MirrorConfig>();
        var urlApi = UrlConst.API_TRAKTEER_PARSER;

        if (config.UseCustomTrakteerApi)
            urlApi = config.TrakteerVerificationApi;

        return urlApi;
    }

    private string GetSaweriaApi()
    {
        var config = appSettingRepository.GetRequiredConfigSection<MirrorConfig>();
        var urlApi = UrlConst.API_SAWERIA_PARSER;

        if (config.UseCustomSaweriaApi)
            urlApi = config.SaweriaVerificationApi;

        return urlApi;
    }
}