using Microsoft.Extensions.Logging;
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
    private readonly FathimahApiService _fathimahApiService;
    private readonly ChatSettingRepository _chatSettingRepository;

    public SendShalatTimeHandler(ILogger<SendShalatTimeHandler> logger, AppSettingRepository appSettingRepository,
        FathimahApiService fathimahApiService, ChatSettingRepository chatSettingRepository)
    {
        _logger = logger;
        _appSettingRepository = appSettingRepository;
        _fathimahApiService = fathimahApiService;
        _chatSettingRepository = chatSettingRepository;
    }

    public async Task<bool> Handle(SendShalatTimeRequest request, CancellationToken cancellationToken)
    {
        var botMain = await _appSettingRepository.GetBotMain();
        var botClient = new TelegramBotClient(botMain.Token);

        const string defaultMessage = "Telah masuk waktu <b>{Shalat}</b> untuk wilayah <b>{City}</b> dan sekitarnya.";

        var cityList = await _chatSettingRepository.GetShalatCity(request.ChatId);

        if (cityList.NotEmpty())
        {
            _logger.LogInformation("City list is empty for ChatId: {ChatId}", request.ChatId);
            return false;
        }

        _logger.LogDebug("Found about {Count} city(es) for ChatId: {ChatId}", cityList.Count, request.ChatId);

        foreach (var cityEntity in cityList)
        {
            try
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
            catch (Exception exception)
            {
                if (exception.Message.IsIgnorable())
                {
                    _logger.LogDebug(exception, "Failed to send Salat Time notification to ChatId: {ChatId}", request.ChatId);
                    break;
                }
                else
                {
                    _logger.LogError(exception, "Error occured send Salat Time notification to ChatId: {ChatId}", request.ChatId);
                }
            }
        }

        return true;
    }
}