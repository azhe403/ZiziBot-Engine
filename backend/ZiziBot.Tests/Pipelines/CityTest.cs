using MongoFramework.Linq;
using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class CityTest
{
    private readonly MediatorService _mediatorService;
    private readonly AppSettingRepository _appSettingRepository;
    private readonly ChatDbContext _chatDbContext;
    private readonly FathimahApiService _fathimahApiService;

    public CityTest(MediatorService mediatorService, AppSettingRepository appSettingRepository, ChatDbContext chatDbContext, FathimahApiService fathimahApiService)
    {
        _mediatorService = mediatorService;
        _appSettingRepository = appSettingRepository;
        _chatDbContext = chatDbContext;
        _fathimahApiService = fathimahApiService;
    }

    [Theory]
    [InlineData(712)]
    public async Task AddCityTest(int cityId)
    {
        var botMain = await _appSettingRepository.GetBotMain();

        // Arrange
        await _mediatorService.Send(new AddCityBotRequest()
        {
            BotToken = botMain.Token,
            Message = SampleMessages.CommonMessage,
            ReplyMessage = true,
            CityId = cityId
        });
    }

    [Theory]
    [InlineData("Cilacap")]
    public async Task AddCityByNameTest(string cityName)
    {
        var chatId = SampleMessages.CommonMessage.Chat.Id;
        var botMain = await _appSettingRepository.GetBotMain();
        var cityInfoAll = await _fathimahApiService.GetAllCityAsync();

        var cityInfo = cityInfoAll.Cities
            .WhereIf(cityName.IsNotNullOrEmpty(), kota => kota.Name.Contains(cityName, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();

        var city = await _chatDbContext.City
            .Where(entity => entity.ChatId == chatId)
            .Where(entity => entity.CityId == cityInfo.Id)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (city != null)
        {
            city.Status = (int)EventStatus.Deleted;
            await _chatDbContext.SaveChangesAsync();
        }

        // Arrange
        await _mediatorService.Send(new AddCityBotRequest()
        {
            BotToken = botMain.Token,
            Message = SampleMessages.CommonMessage,
            ReplyMessage = true,
            CityName = cityName
        });
    }

    [Theory]
    [InlineData("Cilacap")]
    public async Task AddCityByNameAlreadyAddedTest(string cityName)
    {
        var botMain = await _appSettingRepository.GetBotMain();

        // Arrange
        await _mediatorService.Send(new AddCityBotRequest()
        {
            BotToken = botMain.Token,
            Message = SampleMessages.CommonMessage,
            ReplyMessage = true,
            CityName = cityName
        });
    }

    [Theory]
    [InlineData(-1001404591750)]
    public async Task ShalatTimeTest(long chatId)
    {
        await _mediatorService.Send(new SendShalatTimeRequest()
        {
            ChatId = chatId
        });
    }
}