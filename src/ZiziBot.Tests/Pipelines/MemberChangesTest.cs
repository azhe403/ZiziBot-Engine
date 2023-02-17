using Allowed.Telegram.Bot.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
        foreach (var botData in ListBotData)
        {
            await _mediator.EnqueueAsync(
                new NewChatMembersRequestModel()
                {
                    BotToken = botData.Token,
                    Message = SampleMessages.NewChatMembers
                }
            );
        }

        Assert.True(true);
    }
}