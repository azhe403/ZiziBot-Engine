using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class ForwardChannelTest
{
    private readonly MediatorService _mediatorService;
    private readonly AppSettingRepository _appSettingRepository;

    public ForwardChannelTest(MediatorService mediatorService, AppSettingRepository appSettingRepository)
    {
        _mediatorService = mediatorService;
        _appSettingRepository = appSettingRepository;
    }

    [Fact]
    public async Task SendChannelPostTest()
    {
        var botMain = await _appSettingRepository.GetBotMain();

        await _mediatorService.Send(new ForwardChannelPostRequest()
        {
            BotToken = botMain.Token,
            Update = SampleMessages.UpdateChannelPost
        });
    }

    [Fact]
    public async Task SendChannelPostDocumentTest()
    {
        var botMain = await _appSettingRepository.GetBotMain();

        await _mediatorService.Send(new ForwardChannelPostRequest()
        {
            BotToken = botMain.Token,
            Update = SampleMessages.UpdateChannelPostDocument
        });
    }
}