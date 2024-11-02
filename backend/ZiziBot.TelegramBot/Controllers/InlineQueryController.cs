using Microsoft.Extensions.Logging;
using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class InlineQueryController(
    ILogger<InlineQueryController> logger,
    MediatorService mediatorService
) : BotCommandController
{
    [InlineQuery()]
    public async Task InlineSearchBotApi(CommandData data)
    {
        var inlineCmd = data.InlineQuery.Query.GetInlineQueryAt<string>(0);

        var result = inlineCmd switch {
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

    [InlineQuery("api-doc")]
    public async Task SearchApiDoc(CommandData data)
    {
        await mediatorService.EnqueueAsync(new AnswerInlineQueryApiDocBotRequestModel() {
            BotToken = data.BotToken,
            InlineQuery = data.InlineQuery,
            Query = data.InlineQueryQueryParam
        });
    }

    [InlineQuery("anteraja")]
    [InlineQuery("jne")]
    [InlineQuery("jnt")]
    [InlineQuery("lion")]
    [InlineQuery("ncs")]
    [InlineQuery("tiki")]
    [InlineQuery("trawl")]
    [InlineQuery("trawlbens")]
    [InlineQuery("sicepat")]
    [InlineQuery("wahana")]
    public async Task SearchResi(CommandData data)
    {
        await mediatorService.EnqueueAsync(new CheckAwbInlineRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            InlineQuery = data.InlineQuery,
            ReplyMessage = true,
        });
    }

    public async Task WebSearch(CommandData data)
    { }
}