using Allowed.Telegram.Bot.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class PingTest
{
    private readonly IMediator _mediator;
    private readonly IOptionsSnapshot<List<BotData>> _botOptions;

    private List<BotData> ListBotData => _botOptions.Value;

    public PingTest(IMediator mediator, IOptionsSnapshot<List<BotData>> botOptions)
    {
        _mediator = mediator;
        _botOptions = botOptions;
    }

    [Fact]
    public async Task Ping()
    {
        var message = new Message
        {
            Chat = new Chat
            {
                Id = -1001404591750,
                Title = "🇮🇩 Telegram Bot API🔥🔥"
            },
            From = new User
            {
                Id = 1025424321,
                FirstName = "Sandal",
                LastName = "Jepit"
            }
        };

        foreach (var botData in ListBotData)
        {
            await _mediator.EnqueueAsync(new PingRequestModel
            {
                BotData = botData,
                Message = message,
                DirectAction = true
            });
        }
    }
}
