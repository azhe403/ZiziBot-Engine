using Microsoft.Extensions.Logging;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class InlineQueryController(
    ILogger<InlineQueryController> logger,
    MediatorService mediatorService
) : BotCommandController
{
    // [InlineQuery()]
    public async Task InlineSearchBotApi(CommandData data)
    {
        var inlineCmd = data.InlineQuery.Query.GetInlineQueryAt<string>(0);

        var result = inlineCmd switch {
            "api-doc" => await mediatorService.EnqueueAsync(new AnswerInlineQueryApiDocBotRequestModel() {
                BotToken = data.BotToken,
                InlineQuery = data.InlineQuery,
                Query = data.InlineQuery.Query.GetInlineQueryAt<string>(1)
            }),
            "uup" => await mediatorService.EnqueueAsync(new AnswerInlineQueryUupBotRequestModel() {
                BotToken = data.BotToken,
                InlineQuery = data.InlineQuery,
                Query = data.InlineQuery.Query.GetInlineQueryAt<string>(1)
            }),
            "search" => await mediatorService.EnqueueAsync(new AnswerInlineQueryWebSearchBotRequestModel() {
                BotToken = data.BotToken,
                InlineQuery = data.InlineQuery,
                Query = data.InlineQuery.Query.Replace(inlineCmd, "").Trim()
            }),
            "subdl" => await mediatorService.EnqueueAsync(new AnswerInlineQuerySubdlRequest() {
                BotToken = data.BotToken,
                InlineQuery = data.InlineQuery
            }),
            _ => await mediatorService.EnqueueAsync(new AnswerInlineQueryGuideBotRequestModel() {
                BotToken = data.BotToken,
                InlineQuery = data.InlineQuery
            })
        };

        logger.LogInformation("InlineSearchBotApi: {@Result}", result);
    }
}