using MongoFramework.Linq;
using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class MirrorUserTest
{
    private readonly MediatorService _mediatorService;
    private readonly AppSettingRepository _appSettingRepository;
    private readonly MongoDbContextBase _mongoDbContext;

    public MirrorUserTest(MediatorService mediatorService, AppSettingRepository appSettingRepository, MongoDbContextBase mongoDbContext)
    {
        _mediatorService = mediatorService;
        _appSettingRepository = appSettingRepository;
        _mongoDbContext = mongoDbContext;
    }

    [Theory]
    [InlineData("ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")] // trakteer
    public async Task SubmitTrakteerPaymentTest(string url)
    {
        // Arrange
        var bot = await _appSettingRepository.GetBotMain();
        var payment = await _mongoDbContext.MirrorApproval
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(entity => entity.PaymentUrl == url);

        if (payment != null)
        {
            payment.Status = (int)EventStatus.Deleted;

            await _mongoDbContext.SaveChangesAsync();
        }

        // Assert
        Assert.NotNull(bot);

        await _mediatorService.Send(new SubmitPaymentBotRequestModel()
        {
            BotToken = bot.Token,
            Message = SampleMessages.CommonMessage,
            Payload = url
        });
    }

    [Theory]
    [InlineData("ca9c28da-87c0-5d32-9b6f-a220d3d36dfd", 12345)] // trakteer
    public async Task SubmitTrakteerPaymentForUserIdTest(string url, long userId)
    {
        // Arrange
        var bot = await _appSettingRepository.GetBotMain();
        var payment = await _mongoDbContext.MirrorApproval
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(entity => entity.PaymentUrl == url);

        if (payment != null)
        {
            payment.Status = (int)EventStatus.Deleted;

            await _mongoDbContext.SaveChangesAsync();
        }

        // Assert
        Assert.NotNull(bot);

        await _mediatorService.Send(new SubmitPaymentBotRequestModel()
        {
            BotToken = bot.Token,
            Message = SampleMessages.CommonMessage,
            Payload = url,
            ForUserId = userId
        });
    }

    [Theory]
    [InlineData("ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")] // trakteer
    public async Task SubmitTrakteerPaymentConfirmationExpiredTest(string url)
    {
        // Arrange
        var bot = await _appSettingRepository.GetBotMain();
        await _appSettingRepository.UpdateAppSetting("Mirror:PaymentExpirationDays", "3");

        Assert.NotNull(bot);

        await _mediatorService.Send(new SubmitPaymentBotRequestModel()
        {
            BotToken = bot.Token,
            Message = SampleMessages.CommonMessage,
            Payload = url
        });

        await _appSettingRepository.UpdateAppSetting("Mirror:PaymentExpirationDays", "300");
    }

    [Theory]
    [InlineData("ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")] // trakteer
    public async Task SubmitTrakteerPaymentAlreadyPaidTest(string url)
    {
        // Arrange
        var bot = await _appSettingRepository.GetBotMain();

        Assert.NotNull(bot);

        await _mediatorService.Send(new SubmitPaymentBotRequestModel()
        {
            BotToken = bot.Token,
            Message = SampleMessages.CommonMessage,
            Payload = url
        });
    }

    [Theory]
    [InlineData("dummy-order-id")]
    public async Task SubmitTrakteerPaymentInvalidOrderIdTest(string url)
    {
        // Arrange
        var bot = await _appSettingRepository.GetBotMain();

        Assert.NotNull(bot);

        await _mediatorService.Send(new SubmitPaymentBotRequestModel()
        {
            BotToken = bot.Token,
            Message = SampleMessages.CommonMessage,
            Payload = url
        });
    }
}