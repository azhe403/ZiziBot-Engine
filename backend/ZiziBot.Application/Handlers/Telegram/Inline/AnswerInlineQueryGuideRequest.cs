using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Inline;

public class AnswerInlineQueryGuideBotRequestModel : BotRequestBase
{ }

public class AnswerInlineQueryGuideRequestHandler(
    ServiceFacade serviceFacade
)
    : IBotRequestHandler<AnswerInlineQueryGuideBotRequestModel>
{
    public async Task<BotResponseBase> Handle(AnswerInlineQueryGuideBotRequestModel request, CancellationToken cancellationToken)
    {
        serviceFacade.TelegramService.SetupResponse(request: request);

        return await serviceFacade.TelegramService.AnswerInlineQueryAsync(new List<InlineQueryResult>() {
            new InlineQueryResultArticle(
                id: "guide-1",
                title: "Bagaimana cara menggunakannya?",
                inputMessageContent: new InputTextMessageContent(InlineDefaults.DefaultGuideText) {
                    LinkPreviewOptions = true
                }
            ) {
                ReplyMarkup = InlineDefaults.DefaultButtonMarkup
            },
            new InlineQueryResultArticle(
                id: "guide-2",
                title: "Cobalah ketikkan 'ping'",
                inputMessageContent: new InputTextMessageContent(InlineDefaults.DefaultGuideText) {
                    LinkPreviewOptions = true
                }
            ) {
                ReplyMarkup = InlineDefaults.DefaultButtonMarkup
            }
        });
    }
}

public static class InlineDefaults
{
    public const string InlineQueryDoc = "https://docs.zizibot.azhe.my.id/features/inline-query";

    public readonly static InlineKeyboardMarkup DefaultButtonMarkup = new(new[] {
        new[] {
            InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Ping", switchInlineQueryCurrentChat: $"ping "),
            InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Api doc", switchInlineQueryCurrentChat: $"api-doc ")
        },
        new[] {
            InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Web Search", switchInlineQueryCurrentChat: $"search "),
            InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Windows UUPs", switchInlineQueryCurrentChat: $"uup ")
        }
    });

    public readonly static string DefaultGuideText = $"Silakan pelajari selengkapnya" +
                                                     $"\n{InlineQueryDoc}" +
                                                     $"\n\nAtau tekan salah satu tombol dibawah ini";
}