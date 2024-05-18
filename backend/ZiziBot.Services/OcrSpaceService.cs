using Flurl.Http;
using ZiziBot.Contracts.Enums;
using ZiziBot.Types.Vendor.OcrSpace;

namespace ZiziBot.Services;

public class OcrSpaceService(IApiKeyService apiKeyService)
{
    public async Task<string> ParseImage(string fileName)
    {
        var apiKey = await apiKeyService.GetApiKeyAsync(ApiKeyCategory.Internal, ApiKeyVendor.OcrSpace);

        if (apiKey.IsNullOrEmpty())
        {
            return "OCR belum dipersiapkan";
        }

        var response = await UrlConst.OCR_SPACE_URL_API.PostMultipartAsync(content => {
            content.AddFile("file", fileName);
            content.Headers.Add("apikey", apiKey);
        });

        var json = await response.GetJsonAsync<OcrSpaceRoot>();

        return json.ParsedResults.Select(x => x.ParsedText).StrJoin("\n");
    }
}