using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Inline;

public class AnswerInlineQueryGuideRequestModel : RequestBase
{

}

public class AnswerInlineQueryGuideRequestHandler : IRequestHandler<AnswerInlineQueryGuideRequestModel, ResponseBase>
{
    private readonly TelegramService _telegramService;

    public AnswerInlineQueryGuideRequestHandler(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<ResponseBase> Handle(AnswerInlineQueryGuideRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var learnMore = "https://docs.zizibot.winten.my.id/features/inline-query";
        var learnMoreContent = $"Silakan pelajari selengkapnya" +
                               $"\n{learnMore}" +
                               $"\n\nAtau tekan salah satu tombol dibawah ini";

        var replyMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Ping", $"ping"),
                InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Api doc", $"api-doc")
            }
        });

        return await _telegramService.AnswerInlineQueryAsync(new List<InlineQueryResult>()
        {
            new InlineQueryResultArticle(
                id: "guide-1",
                title: "Bagaimana cara menggunakannya?",
                inputMessageContent: new InputTextMessageContent(learnMoreContent)
                {
                    DisableWebPagePreview = true
                }
            )
            {
                ReplyMarkup = replyMarkup
            },
            new InlineQueryResultArticle(
                id: "guide-2",
                title: "Cobalah ketikkan 'ping'",
                inputMessageContent: new InputTextMessageContent(learnMoreContent)
                {
                    DisableWebPagePreview = true
                }
            )
            {
                ReplyMarkup = replyMarkup
            }
        });
    }

}