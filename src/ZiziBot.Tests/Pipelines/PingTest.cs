using Allowed.Telegram.Bot.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class PingTest
{
    private readonly IMediator _mediator;
    private readonly IOptionsSnapshot<List<SimpleTelegramBotClientOptions>> _botOptions;

    private List<SimpleTelegramBotClientOptions> ListBotData => _botOptions.Value;

    public PingTest(IMediator mediator, IOptionsSnapshot<List<SimpleTelegramBotClientOptions>> botOptions)
    {
        _mediator = mediator;
        _botOptions = botOptions;
    }

    [Fact]
    public async Task Ping()
    {
        foreach (var botData in ListBotData)
        {
            await _mediator.EnqueueAsync(
                new PingRequestModel
                {
                    BotToken = botData.Token,
                    Message = SampleMessages.CommonMessage
                }
            );
        }

        Assert.True(true);
    }
}