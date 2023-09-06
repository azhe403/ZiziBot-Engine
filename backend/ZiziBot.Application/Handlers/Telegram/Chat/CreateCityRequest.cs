using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class AddCityBotRequest : BotRequestBase
{
    public int CityId { get; set; }
    public string? CityName { get; set; }
}

internal class AddCityHandler : IRequestHandler<AddCityBotRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly FathimahApiService _fathimahApiService;

    public AddCityHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext, FathimahApiService fathimahApiService)
    {
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
        _fathimahApiService = fathimahApiService;
    }

    public async Task<BotResponseBase> Handle(AddCityBotRequest request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        _telegramService.SetupResponse(request);

        var cityInfoAll = await _fathimahApiService.GetAllCityAsync();
        var cityInfo = cityInfoAll.Cities
            .WhereIf(request.CityId > 0, kota => kota.Id == request.CityId)
            .WhereIf(request.CityName.IsNotNullOrEmpty(), kota => kota.Name.Contains(request.CityName, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();

        if (cityInfo == null)
        {
            return await _telegramService.SendMessageText("Kota tidak ditemukan");
        }

        var city = await _mongoDbContext.City
            .Where(entity => entity.ChatId == request.ChatIdentifier)
            .Where(entity => entity.CityId == cityInfo.Id)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var cityMsg = HtmlMessage.Empty
            .Bold("ID Kota: ").CodeBr(cityInfo.Id.ToString())
            .Bold("Kota: ").CodeBr(cityInfo.Name);

        if (city != null)
        {
            htmlMessage.Text("Kota sudah ditambahkan")
                .Br()
                .Append(cityMsg);
        }
        else
        {
            _mongoDbContext.City.Add(new CityEntity()
            {
                ChatId = request.ChatIdentifier,
                UserId = request.UserId,
                CityId = cityInfo.Id,
                CityName = cityInfo.Name,
                Status = (int)EventStatus.Complete
            });

            await _mongoDbContext.SaveChangesAsync(cancellationToken);

            htmlMessage.Text("Kota berhasil disimpan")
                .Br()
                .Append(cityMsg);
        }

        return await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}