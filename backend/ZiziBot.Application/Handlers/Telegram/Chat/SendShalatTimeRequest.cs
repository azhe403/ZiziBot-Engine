using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class SendShalatTimeRequest : IRequest<bool>
{
    public long ChatId { get; set; }
}

public class SendShalatTimeHandler : IRequestHandler<SendShalatTimeRequest, bool>
{
    private readonly ILogger<SendShalatTimeHandler> _logger;
    private readonly AppSettingRepository _appSettingRepository;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly FathimahApiService _fathimahApiService;

    public SendShalatTimeHandler(ILogger<SendShalatTimeHandler> logger, AppSettingRepository appSettingRepository, MongoDbContextBase mongoDbContext,
        FathimahApiService fathimahApiService)
    {
        _logger = logger;
        _appSettingRepository = appSettingRepository;
        _mongoDbContext = mongoDbContext;
        _fathimahApiService = fathimahApiService;
    }

    public async Task<bool> Handle(SendShalatTimeRequest request, CancellationToken cancellationToken)
    {
        var botMain = await _appSettingRepository.GetBotMain();
        var botClient = new TelegramBotClient(botMain.Token);

        var defaultMessage = "Telah masuk waktu <b>{Shalat}</b> untuk wilayah <b>{City}</b> dan sekitarnya.";

        var cityList = await _mongoDbContext.City
            .Where(entity => entity.ChatId == request.ChatId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .OrderBy(entity => entity.CityName)
            .ToListAsync(cancellationToken: cancellationToken);

        _logger.LogDebug("Found about {Count} city(es)", cityList.Count);

        if (!cityList.Any())
        {
            _logger.LogInformation("City list is empty");
            return false;
        }

        foreach (var cityEntity in cityList)
        {
            var currentTime = DateTime.UtcNow.AddHours(Env.DEFAULT_TIMEZONE).ToString("HH:mm");
            var shalatTime = await _fathimahApiService.GetShalatTime(DateTime.Now, cityEntity.CityId);
            var currentShalat = shalatTime.Schedule.ShalatDict.FirstOrDefault(pair => pair.Value == currentTime);

            if (currentShalat.IsNull())
            {
                _logger.LogDebug("No match Shalat time for city: '{CityName}' at '{CurrentTime}'", cityEntity.CityName, currentTime);
                continue;
            }

            var message = defaultMessage.ResolveVariable(new List<(string placeholder, string value)>
            {
                ("Shalat", currentShalat.Key),
                ("Date", DateTime.UtcNow.AddHours(Env.DEFAULT_TIMEZONE).ToString("yyyy-MM-dd HH:mm:ss")),
                ("City", cityEntity.CityName)
            });

            await botClient.SendTextMessageAsync(request.ChatId, message, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
        }

        return true;
    }
}