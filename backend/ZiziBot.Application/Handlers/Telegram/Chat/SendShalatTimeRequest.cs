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
    private readonly ChatDbContext _chatDbContext;
    private readonly FathimahApiService _fathimahApiService;

    public SendShalatTimeHandler(ILogger<SendShalatTimeHandler> logger, AppSettingRepository appSettingRepository, ChatDbContext chatDbContext, FathimahApiService fathimahApiService)
    {
        _logger = logger;
        _appSettingRepository = appSettingRepository;
        _chatDbContext = chatDbContext;
        _fathimahApiService = fathimahApiService;
    }

    public async Task<bool> Handle(SendShalatTimeRequest request, CancellationToken cancellationToken)
    {
        var botMain = await _appSettingRepository.GetBotMain();
        var botClient = new TelegramBotClient(botMain.Token);

        var cityList = await _chatDbContext.City
            .Where(entity => entity.ChatId == request.ChatId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .OrderBy(entity => entity.CityName)
            .ToListAsync(cancellationToken: cancellationToken);
        if (!cityList.Any())
        {
            _logger.LogInformation("City list is empty");
            return false;
        }

        foreach (var cityEntity in cityList)
        {
            var currentTime = DateTime.UtcNow.AddHours(Env.DEFAULT_TIMEZONE).ToString("HH:mm");
            var shalatTime = await _fathimahApiService.GetShalatTime(DateOnly.FromDateTime(DateTime.Now), cityEntity.CityId);
            var currentShalat = shalatTime.Schedule.ShalatDict.FirstOrDefault(pair => pair.Value == currentTime);

            if (currentShalat.IsNull())
            {
                _logger.LogDebug("No match Shalat time for city: '{CityName}' at '{CurrentTime}'", cityEntity.CityName, currentTime);
                return true;
            }

            var htmlMessage = HtmlMessage.Empty;
            htmlMessage.Text($"Telah masuk waktu ").Bold(currentShalat.Key).Text(" untuk wilayah ").Bold(cityEntity.CityName).Text(" dan sekitarnya.").Br();

            await botClient.SendTextMessageAsync(request.ChatId, htmlMessage.ToString(), parseMode: ParseMode.Html, cancellationToken: cancellationToken);
        }

        return true;
    }
}