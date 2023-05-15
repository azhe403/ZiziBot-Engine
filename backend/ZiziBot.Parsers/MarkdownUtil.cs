using Markdig;

namespace ZiziBot.Parsers;

public static class MarkdownUtil
{
    public static string MdToHtml(this string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        return Markdown.ToHtml(markdown, pipeline);
    }
}