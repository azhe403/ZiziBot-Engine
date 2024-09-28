using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class GetCityListBotRequest : BotRequestBase
{
}

public class GetCityListHandler(ILogger<GetCityListHandler> logger, TelegramService telegramService, MongoDbContextBase mongoDbContext)
    : IRequestHandler<GetCityListBotRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(GetCityListBotRequest request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        telegramService.SetupResponse(request);

        logger.LogDebug("Getting city list from chat {ChatId}", request.ChatId);

        var cityList = await mongoDbContext.BangHasan_ShalatCity
            .Where(entity => entity.ChatId == request.ChatIdentifier)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .OrderBy(entity => entity.CityName)
            .ToListAsync(cancellationToken: cancellationToken);

        if (cityList.Any())
        {
            htmlMessage.Bold("Daftar Kota").Br();

            foreach (var city in cityList)
            {
                htmlMessage.Code(city.CityId.ToString()).Text(" - ").Text(city.CityName).Br();
            }
        }
        else
        {
            htmlMessage.BoldBr("Daftar Kota Kosong");
        }

        return await telegramService.SendMessageText(htmlMessage.ToString());
    }
}