using Allowed.Telegram.Bot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class MemberChangesTest
{
    private readonly MediatorService _mediatorService;
    private readonly IConfiguration _configuration;
    private readonly IOptionsSnapshot<List<SimpleTelegramBotClientOptions>> _botOptions;

    private List<SimpleTelegramBotClientOptions> ListBotData => _botOptions.Value;

    public MemberChangesTest(MediatorService mediatorService, IOptionsSnapshot<List<SimpleTelegramBotClientOptions>> botOptions, IConfiguration configuration)
    {
        _mediatorService = mediatorService;
        _botOptions = botOptions;
        _configuration = configuration;
    }

    [Fact]
    public async Task NewChatMembers()
    {
        foreach (var botData in ListBotData)
        {
            await _mediatorService.EnqueueAsync(
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