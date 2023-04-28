using Allowed.Telegram.Bot.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class PingTest
{
    private readonly MediatorService _mediatorService;
    private readonly IOptionsSnapshot<List<SimpleTelegramBotClientOptions>> _botOptions;

    private List<SimpleTelegramBotClientOptions> ListBotData => _botOptions.Value;

    public PingTest(MediatorService mediatorService, IOptionsSnapshot<List<SimpleTelegramBotClientOptions>> botOptions)
    {
        _mediatorService = mediatorService;
        _botOptions = botOptions;
    }

    [Fact]
    public async Task Ping()
    {
        foreach (var botData in ListBotData)
        {
            await _mediatorService.EnqueueAsync(new PingRequestModel
            {
                BotToken = botData.Token,
                Message = SampleMessages.CommonMessage
            });
        }

        Assert.True(true);
    }
}