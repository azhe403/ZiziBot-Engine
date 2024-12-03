using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("JadwalSholatOrg.ChatCity")]
public class JadwalSholatOrg_ChatCityEntity : EntityBase
{
    public int CityId { get; set; }
    public int ChatId { get; set; }
}