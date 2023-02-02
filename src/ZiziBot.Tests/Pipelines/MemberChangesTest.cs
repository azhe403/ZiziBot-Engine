using Allowed.Telegram.Bot.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class MemberChangesTest
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;
    private readonly IOptionsSnapshot<List<SimpleTelegramBotClientOptions>> _botOptions;

    private List<SimpleTelegramBotClientOptions> ListBotData => _botOptions.Value;

    public MemberChangesTest(IMediator mediator, IOptionsSnapshot<List<SimpleTelegramBotClientOptions>> botOptions, IConfiguration configuration)
    {
        _mediator = mediator;
        _botOptions = botOptions;
        _configuration = configuration;
    }

    [Fact]
    public async Task NewChatMembers()
    {
        var message = new Message()
        {
            Chat = new Chat()
            {
                Id = -1001404591750,
                Title = "🇮🇩 Telegram Bot API🔥🔥"
            },
            From = new User()
            {
                Id = 1025424321,
                FirstName = "Sandal",
                LastName = "Jepit"
            },
            NewChatMembers = new[]
            {
                new User()
                {
                    Id = 1025424321,
                    FirstName = "Sandal",
                    LastName = "Jepit"
                },
                new User()
                {
                    Id = 1025424321,
                    FirstName = "Fulan",
                    LastName = "Ahmad"
                },
                new User()
                {
                    Id = 552609163,
                    FirstName = "I'm",
                    LastName = "Groot"
                }
            }
        };

        foreach (var botData in ListBotData)
        {
            await _mediator.EnqueueAsync(
                new NewChatMembersRequestModel()
                {
                    BotToken = botData.Token,
                    Message = message
                }
            );
        }

        Assert.True(true);
    }
}