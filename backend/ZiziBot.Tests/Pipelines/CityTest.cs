using Microsoft.EntityFrameworkCore;
using Xunit;
using ZiziBot.DataSource.MongoEf;

namespace ZiziBot.Tests.Pipelines;

public class CityTest(MediatorService mediatorService, AppSettingRepository appSettingRepository, MongoEfContext mongoEfContext, FathimahApiService fathimahApiService)
{
    [Theory]
    [InlineData(712)]
    public async Task AddCityTest(int cityId)
    {
        var botMain = await appSettingRepository.GetBotMain();

        // Arrange
        await mediatorService.Send(new AddCityBotRequest() {
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
        var botMain = await appSettingRepository.GetBotMain();
        var cityInfoAll = await fathimahApiService.GetAllCityAsync();

        var cityInfo = cityInfoAll.Cities
            .WhereIf(cityName.IsNotNullOrEmpty(), kota => kota.Lokasi.Contains(cityName, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();

        var city = await mongoEfContext.BangHasan_ShalatCity
            .Where(entity => entity.ChatId == chatId)
            .Where(entity => entity.CityId == cityInfo.Id)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (city != null)
        {
            city.Status = (int)EventStatus.Deleted;
            await mongoEfContext.SaveChangesAsync();
        }

        // Arrange
        await mediatorService.Send(new AddCityBotRequest() {
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
        var botMain = await appSettingRepository.GetBotMain();

        // Arrange
        await mediatorService.Send(new AddCityBotRequest() {
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
        await mediatorService.Send(new SendShalatTimeRequest() {
            ChatId = chatId
        });
    }
}