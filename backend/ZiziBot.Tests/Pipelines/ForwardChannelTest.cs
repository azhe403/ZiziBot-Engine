using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class ForwardChannelTest(MediatorService mediatorService, BotRepository botRepository)
{
    [Fact]
    public async Task SendChannelPostTest()
    {
        var botMain = await botRepository.GetBotMain();

        await mediatorService.Send(new ForwardChannelPostRequest()
        {
            BotToken = botMain.Token,
            Update = SampleMessages.UpdateChannelPost
        });
    }

    [Fact]
    public async Task SendChannelPostDocumentTest()
    {
        var botMain = await botRepository.GetBotMain();

        await mediatorService.Send(new ForwardChannelPostRequest()
        {
            BotToken = botMain.Token,
            Update = SampleMessages.UpdateChannelPostDocument
        });
    }
}