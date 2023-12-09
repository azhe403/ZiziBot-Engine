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

    public static async Task<IDocument> OpenHtml(this string htmlString)
    {
        var config = Configuration.Default;

        //Create a new context for evaluating webpages with the given config
        var context = BrowsingContext.New(config);

        //Parse the document from the content of a response to a virtual request
        var document = await context.OpenAsync(req => req.Content(htmlString));

        return document;
    }

    public static TElement? QuerySelector<TElement>(this IDocument? document, string selector) where TElement : class
    {
        return document?.QuerySelector(selector) as TElement;
    }

    public static IEnumerable<TElement>? QuerySelectorAll<TElement>(this IDocument? document, string selector) where TElement : class
    {
        return document?.QuerySelectorAll(selector).Cast<TElement>();
    }
}