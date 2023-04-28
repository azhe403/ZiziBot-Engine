using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class MirrorUserTest
{
    private readonly MediatorService _mediatorService;
    private readonly AppSettingRepository _appSettingRepository;

    public MirrorUserTest(MediatorService mediatorService, AppSettingRepository appSettingRepository)
    {
        _mediatorService = mediatorService;
        _appSettingRepository = appSettingRepository;
    }

    [Theory]
    [InlineData("https://trakteer.id/payment-status/ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")]
    public async Task SubmitPaymentTest(string url)
    {
        var bot = await _appSettingRepository.GetBotMain();

        Assert.NotNull(bot);

        await _mediatorService.Send(new SubmitPaymentRequestModel()
        {
            BotToken = bot.Token,
            Message = SampleMessages.CommonMessage,
            Payload = url
        });
    }
}