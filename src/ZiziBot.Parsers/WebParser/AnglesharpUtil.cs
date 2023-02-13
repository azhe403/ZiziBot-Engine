using AngleSharp;
using AngleSharp.Dom;

namespace ZiziBot.Parsers.WebParser;

public static class AnglesharpUtil
{
    public static async Task<IDocument> OpenUrl(this string url)
    {
        var config = Configuration.Default
            .WithDefaultLoader()
            .WithDefaultCookies();

        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(url);

        return document;
    }
}