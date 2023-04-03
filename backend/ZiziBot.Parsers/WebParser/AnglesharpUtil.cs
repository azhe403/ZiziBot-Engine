using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io.Network;

namespace ZiziBot.Parsers.WebParser;

public static class AnglesharpUtil
{
    public static async Task<IDocument> OpenUrl(this string url)
    {
        var httpClient = new HttpClient();
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