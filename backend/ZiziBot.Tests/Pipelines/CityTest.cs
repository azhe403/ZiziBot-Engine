using Microsoft.EntityFrameworkCore;
using Xunit;
using ZiziBot.Common.Enums;
using ZiziBot.Common.Utils;
using ZiziBot.Database.MongoDb;
using ZiziBot.Services.Rest;

namespace ZiziBot.Tests.Pipelines;

public class CityTest(MediatorService mediatorService, BotRepository botRepository, MongoDbContext mongoDbContext, FathimahRestService fathimahRestService)
{
    [Theory]
    [InlineData(712)]
    public async Task AddCityTest(int cityId)
    {
        var botMain = await botRepository.GetBotMain();

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
        var botMain = await botRepository.GetBotMain();
        var cityInfoAll = await fathimahRestService.GetAllCityAsync();

        var cityInfo = cityInfoAll.Cities
            .WhereIf(cityName.IsNotNullOrEmpty(), kota => kota.Lokasi.Contains(cityName, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();

        var city = await mongoDbContext.BangHasan_ShalatCity
            .Where(entity => entity.ChatId == chatId)
            .Where(entity => entity.CityId == cityInfo.Id)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (city != null)
        {
            city.Status = (int)EventStatus.Deleted;
            await mongoDbContext.SaveChangesAsync();
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
        var botMain = await botRepository.GetBotMain();

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