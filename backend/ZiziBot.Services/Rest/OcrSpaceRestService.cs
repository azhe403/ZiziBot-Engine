using Flurl.Http;
using ZiziBot.Common.Enums;
using ZiziBot.Common.Vendor.OcrSpace;

namespace ZiziBot.Services.Rest;

public class OcrSpaceRestService(BotRepository botRepository)
{
    public async Task<string?> ParseImage(string fileName)
    {
        var apiKey = await botRepository.GetApiKeyAsync(ApiKeyCategory.Internal, ApiKeyVendor.OcrSpace);

        if (apiKey.IsNullOrEmpty())
        {
            return "OCR belum dipersiapkan";
        }

        var response = await UrlConst.OCR_SPACE_URL_API.PostMultipartAsync(content => {
            content.AddFile("file", fileName);
            content.Headers.Add("apikey", apiKey);
        });

        var json = await response.GetJsonAsync<OcrSpaceRoot>();

        return json.ParsedResults?.Select(x => x.ParsedText).StrJoin("\n").Trim();
    }
}