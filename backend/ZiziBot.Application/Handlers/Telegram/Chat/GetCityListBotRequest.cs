using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class GetCityListBotRequest : BotRequestBase
{
}

public class GetCityListHandler : IRequestHandler<GetCityListBotRequest, BotResponseBase>
{
    private readonly ILogger<GetCityListHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;

    public GetCityListHandler(ILogger<GetCityListHandler> logger, TelegramService telegramService, MongoDbContextBase mongoDbContext)
    {
        _logger = logger;
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<BotResponseBase> Handle(GetCityListBotRequest request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        _telegramService.SetupResponse(request);

        _logger.LogDebug("Getting city list from chat {ChatId}", request.ChatId);

        var cityList = await _mongoDbContext.City
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

        return await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}