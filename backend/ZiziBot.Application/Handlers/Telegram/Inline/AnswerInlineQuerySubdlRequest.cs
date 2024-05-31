using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using ZiziBot.Types.Vendor.Subdl;

namespace ZiziBot.Application.Handlers.Telegram.Inline;

public class AnswerInlineQuerySubdlRequest : BotRequestBase
{
}

public class AnswerInlineQuerySubdlHandler(ILogger<AnswerInlineQuerySubdlHandler> logger, TelegramService telegramService, SubdlService subdlService)
    : IBotRequestHandler<AnswerInlineQuerySubdlRequest>
{
    public async Task<BotResponseBase> Handle(AnswerInlineQuerySubdlRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);
        Popular popular = new();
        IEnumerable<InlineQueryResult>? inlineQueryResults = default;

        if (request.InlineParam.IsNullOrEmpty())
        {
            logger.LogInformation("Find subdl for Query: {query}", request.InlineParam);
            popular = await subdlService.FetchPopular();
        }

        inlineQueryResults = popular.Results?.Select(x => {
            var subtitleUrl = $"https://subdl.com/subtitle/{x.SdId}/{x.Slug}";
            var htmlContent = HtmlMessage.Empty
                .CodeBr(x.Name)
                .TextBr(subtitleUrl);

            var replyMarkup = new InlineKeyboardMarkup(new[] {
                new[] {
                    InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Pencarian baru", $"subdl "),
                    InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Pencarian lanjut", $"subdl {request.InlineParam}")
                }
            });

            var fields = new InlineQueryResultArticle(
                id: $"subdl/{x.SdId}/{x.Slug}",
                title: x.Name,
                inputMessageContent: new InputTextMessageContent(htmlContent.ToString()) {
                    ParseMode = ParseMode.Html,
                    DisableWebPagePreview = false
                }
            ) {
                Description = x.Slug,
                ReplyMarkup = replyMarkup
            };

            return fields;
        });


        return await telegramService.AnswerInlineQueryAsync(inlineQueryResults);
    }
}