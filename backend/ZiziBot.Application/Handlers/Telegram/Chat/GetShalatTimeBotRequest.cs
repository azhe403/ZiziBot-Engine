using System.Globalization;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using MoreLinq;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class GetShalatTimeBotRequest : BotRequestBase
{
}

public class GetShalatTimeHandler : IRequestHandler<GetShalatTimeBotRequest, BotResponseBase>
{
    private readonly ILogger<GetShalatTimeHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly FathimahApiService _fathimahApiService;

    public GetShalatTimeHandler(ILogger<GetShalatTimeHandler> logger, TelegramService telegramService,
        MongoDbContextBase mongoDbContext, FathimahApiService fathimahApiService)
    {
        _logger = logger;
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
        _fathimahApiService = fathimahApiService;
    }

    public async Task<BotResponseBase> Handle(GetShalatTimeBotRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get Shalat Time list for ChatId: {ChatId}", request.ChatId);
        _telegramService.SetupResponse(request);

        var cityList = await _mongoDbContext.BangHasan_ShalatCity
            .Where(entity => entity.ChatId == request.ChatIdentifier)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .OrderBy(entity => entity.CityName)
            .ToListAsync(cancellationToken: cancellationToken);

        var htmlMessage = HtmlMessage.Empty;

        if (cityList.Any())
        {
            htmlMessage.Bold("Daftar Waktu Shalat").Br()
                .TextBr(DateTime.UtcNow.AddHours(Env.DEFAULT_TIMEZONE).ToString("dddd, dd MMMM yyyy HH:mm:ss",
                    new CultureInfo(request.UserLanguageCode)))
                .TextBr("====================================").Br();

            foreach (var city in cityList)
            {
                htmlMessage.Code(city.CityId.ToString()).Text(" - ").Text(city.CityName).Br()
                    .TextBr("====================================");

                var shalatTime = await _fathimahApiService.GetShalatTime(city.CityId, true);
                shalatTime.Schedule.ShalatDict.ForEach(shalat => {
                    htmlMessage.Bold(shalat.Key).Text(" : ").Text(shalat.Value).Br();
                });

                htmlMessage.Br();
            }
        }
        else
        {
            htmlMessage.BoldBr("Belum ada Kota yg ditambahkan untuk Obrolan ini.");
        }

        return await _telegramService.SendMessageText(htmlMessage);
    }
}