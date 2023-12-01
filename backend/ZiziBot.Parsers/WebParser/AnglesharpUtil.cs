using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io.Network;
using CloudflareSolverRe;

namespace ZiziBot.Parsers.WebParser;

public static class AnglesharpUtil
{
    public static async Task<IDocument?> OpenUrl(this string url, ClearanceHandler? clearanceHandler = null)
    {
        var httpClient = new HttpClient();
        if (clearanceHandler != null)
        {
            httpClient = new HttpClient(clearanceHandler);
        }

        httpClient.DefaultRequestHeaders.Add("User-Agent", Env.COMMON_UA);

        var config = Configuration.Default
            .With(new HttpClientRequester(httpClient))
            .WithJs()
            .WithTemporaryCookies()
            .WithDefaultLoader();

        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(url);

        return document;
    }
}