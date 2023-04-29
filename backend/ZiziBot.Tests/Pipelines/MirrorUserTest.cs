using MongoFramework.Linq;
using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class MirrorUserTest
{
    private readonly MediatorService _mediatorService;
    private readonly AppSettingRepository _appSettingRepository;
    private readonly MirrorDbContext _mirrorDbContext;

    public MirrorUserTest(MediatorService mediatorService, AppSettingRepository appSettingRepository, MirrorDbContext mirrorDbContext)
    {
        _mediatorService = mediatorService;
        _appSettingRepository = appSettingRepository;
        _mirrorDbContext = mirrorDbContext;
    }

    [Theory]
    [InlineData("https://trakteer.id/payment-status/ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")]
    public async Task SubmitTrakteerPaymentTest(string url)
    {
        // Arrange
        var bot = await _appSettingRepository.GetBotMain();
        var payment = await _mirrorDbContext.MirrorApproval
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(entity => entity.PaymentUrl == url);

        if (payment != null)
        {
            payment.Status = (int)EventStatus.Deleted;

            await _mirrorDbContext.SaveChangesAsync();
        }

        // Assert
        Assert.NotNull(bot);

        await _mediatorService.Send(new SubmitPaymentRequestModel()
        {
            BotToken = bot.Token,
            Message = SampleMessages.CommonMessage,
            Payload = url
        });
    }

    [Theory]
    [InlineData("https://trakteer.id/payment-status/ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")]
    public async Task SubmitTrakteerPaymentConfirmationExpiredTest(string url)
    {
        // Arrange
        var bot = await _appSettingRepository.GetBotMain();
        await _appSettingRepository.UpdateAppSetting("Mirror:PaymentExpirationDays", "3");

        Assert.NotNull(bot);

        await _mediatorService.Send(new SubmitPaymentRequestModel()
        {
            BotToken = bot.Token,
            Message = SampleMessages.CommonMessage,
            Payload = url
        });

        await _appSettingRepository.UpdateAppSetting("Mirror:PaymentExpirationDays", "300");
    }

    [Theory]
    [InlineData("https://trakteer.id/payment-status/ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")]
    public async Task SubmitTrakteerPaymentAlreadyPaidTest(string url)
    {
        // Arrange
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