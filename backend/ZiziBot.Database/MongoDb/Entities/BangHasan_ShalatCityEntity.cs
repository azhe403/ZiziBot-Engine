using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("BangHasan.ShalatCity")]
public class BangHasan_ShalatCityEntity : EntityBase
{
    public int CityId { get; set; }
    public string CityName { get; set; }
    public bool EnableNotification { get; set; }
    public long UserId { get; set; }
    public long ChatId { get; set; }
}