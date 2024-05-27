using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class AddCityBotRequest : BotRequestBase
{
    public int CityId { get; set; }
    public string? CityName { get; set; }
}

internal class AddCityHandler(
    TelegramService telegramService,
    MongoDbContextBase mongoDbContext,
    FathimahApiService fathimahApiService
) : IRequestHandler<AddCityBotRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(AddCityBotRequest request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        telegramService.SetupResponse(request);

        var cityInfoAll = await fathimahApiService.GetAllCityAsync();
        var cityInfo = cityInfoAll.Cities
            .WhereIf(request.CityId > 0, kota => kota.Id == request.CityId)
            .WhereIf(request.CityName.IsNotNullOrEmpty(), kota => kota.Lokasi.Like(request.CityName))
            .FirstOrDefault();

        if (cityInfo == null)
        {
            return await telegramService.SendMessageText("Kota tidak ditemukan");
        }

        var city = await mongoDbContext.BangHasan_ShalatCity
            .Where(entity => entity.ChatId == request.ChatIdentifier)
            .Where(entity => entity.CityId == cityInfo.Id)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var cityMsg = HtmlMessage.Empty
            .Bold("ID Kota: ").CodeBr(cityInfo.Id.ToString())
            .Bold("Kota: ").CodeBr(cityInfo.Lokasi);

        if (city != null)
        {
            htmlMessage.Text("Kota sudah ditambahkan")
                .Br()
                .Append(cityMsg);
        }
        else
        {
            mongoDbContext.BangHasan_ShalatCity.Add(new BangHasan_ShalatCityEntity() {
                ChatId = request.ChatIdentifier,
                UserId = request.UserId,
                CityId = cityInfo.Id,
                CityName = cityInfo.Lokasi,
                Status = (int)EventStatus.Complete
            });

            await mongoDbContext.SaveChangesAsync(cancellationToken);

            htmlMessage.Text("Kota berhasil disimpan")
                .Br()
                .Append(cityMsg);
        }

        return await telegramService.SendMessageText(htmlMessage.ToString());
    }
}