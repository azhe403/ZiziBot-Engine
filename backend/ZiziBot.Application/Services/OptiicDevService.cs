using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZiziBot.Application.Services;

public class OptiicDevService
{
    private readonly ILogger<OptiicDevService> _logger;
    private readonly IOptionsSnapshot<OptiicDevConfig> _options;

    private OptiicDevConfig OptiicDevConfig => _options.Value;

    public OptiicDevService(ILogger<OptiicDevService> logger, IOptionsSnapshot<OptiicDevConfig> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task<OptiicDevOcrResult> ScanImageAsync(string fileName)
    {
        _logger.LogInformation("Scanning image {fileName}", fileName);

        var apiKey = OptiicDevConfig.ApiKeys.RandomPick();

        var response = await UrlConst.OCR_URL_API.PostMultipartAsync(
            content => {
                content.AddString("apiKey", apiKey)
                    .AddFile("image", fileName);
            }
        );

        var result = await response.GetJsonAsync<OptiicDevOcrResult>();

        return result;
    }
}