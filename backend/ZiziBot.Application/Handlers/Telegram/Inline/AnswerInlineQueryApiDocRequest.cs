using Flurl.Http;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Inline;

public class AnswerInlineQueryApiDocRequestModel : RequestBase
{
    public string? Query { get; set; }
}

public class AnswerInlineQueryApiDocRequestHandler : IRequestHandler<AnswerInlineQueryApiDocRequestModel, ResponseBase>
{
    private readonly ILogger<AnswerInlineQueryApiDocRequestHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly CacheService _cacheService;

    public AnswerInlineQueryApiDocRequestHandler(ILogger<AnswerInlineQueryApiDocRequestHandler> logger, TelegramService telegramService, CacheService cacheService)
    {
        _logger = logger;
        _telegramService = telegramService;
        _cacheService = cacheService;
    }

    public async Task<ResponseBase> Handle(AnswerInlineQueryApiDocRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        _logger.LogInformation("Find api doc for Query: {query}", request.Query);

        var cache = await _cacheService.GetOrSetAsync(
            cacheKey: CacheKey.API_DOC,
            action: async () => {
                var api = await UrlConst.BOT_API_SPEC.GetJsonAsync<TgBotApiDoc>(cancellationToken: cancellationToken);
                return api;
            });

        var filtered = cache.MethodsAndTypes
            .AsQueryable()
            .WhereIf(!string.IsNullOrWhiteSpace(request.Query), x => x.Key.Contains(request.Query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        _logger.LogInformation("Found api-doc about: {count} result(s)", filtered.Count);

        var inlineQueryResults = filtered
            .Take(50)
            .Select(item => {
                var method = item.Value;
                var description = method.Description?.Aggregate((x, y) => x + y);
                var htmlContent = HtmlMessage.Empty;
                htmlContent.Bold(method.Name).Br()
                    .Text(description, true).Br().Br();

                method.Fields?.ForEach(field => {
                    htmlContent.Bold(field.Name).Br()
                        .Text(field.Description, true).Br().Br();
                });

                var replyMarkup = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("Open doc", method.Href.ToString())
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Pencarian baru", $"api-doc"),
                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Pencarian lanjut", $"api-doc {request.Query}")
                    }
                });

                var fields = new InlineQueryResultArticle(
                    id: CacheKey.API_DOC + method.Name,
                    title: method.Name,
                    inputMessageContent: new InputTextMessageContent(htmlContent.ToString())
                    {
                        ParseMode = ParseMode.Html,
                        DisableWebPagePreview = true
                    }
                )
                {
                    Description = description,
                    ReplyMarkup = replyMarkup
                };

                return fields;
            });

        var learnMore = "https://docs.zizibot.winten.my.id/features/inline-query";
        var learnMoreContent = $"Silakan mauskkan nama method/typw";


        return await _telegramService.AnswerInlineQueryAsync(inlineQueryResults);
    }

}