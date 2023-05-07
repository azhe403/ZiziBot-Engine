using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class InlineQueryController : CommandController
{
    private readonly MediatorService _mediatorService;

    public InlineQueryController(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    [InlineQuery()]
    public async Task InlineSearchBotApi(InlineQueryData data)
    {
        var inlineCmd = data.InlineQuery.Query.GetInlineQueryAt<string>(0);

        var result = inlineCmd switch
        {
            "api-doc" => await _mediatorService.EnqueueAsync(new AnswerInlineQueryApiDocRequestModel()
            {
                BotToken = data.Options.Token,
                InlineQuery = data.InlineQuery,
                Query = data.InlineQuery.Query.GetInlineQueryAt<string>(1)
            }),
            "uup" => await _mediatorService.EnqueueAsync(new AnswerInlineQueryUupRequestModel()
            {
                BotToken = data.Options.Token,
                InlineQuery = data.InlineQuery,
                Query = data.InlineQuery.Query.GetInlineQueryAt<string>(1)
            }),
            _ => await _mediatorService.EnqueueAsync(new AnswerInlineQueryGuideRequestModel()
            {
                BotToken = data.Options.Token,
                InlineQuery = data.InlineQuery
            })
        };


    }
}