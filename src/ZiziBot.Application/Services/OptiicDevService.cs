using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Services;

public class OptiicDevService
{
    private readonly ILogger<OptiicDevService> _logger;

    public OptiicDevService(ILogger<OptiicDevService> logger)
    {
        _logger = logger;
    }

    public async Task<OptiicDevOcrResult> ScanImageAsync(string fileName)
    {
        _logger.LogInformation("Scanning image {fileName}", fileName);

        var response = await UrlConst.OCR_URL_API.PostMultipartAsync(
            content => {
                content.AddString("apiKey", "")
                    .AddFile("image", fileName);
            }
        );

        var result = await response.GetJsonAsync<OptiicDevOcrResult>();

        return result;
    }
}