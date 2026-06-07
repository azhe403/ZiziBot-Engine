using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Application.Database.MongoDb.Entities;

[Collection("JadwalSholatOrg.City")]
public class JadwalSholatOrg_CityEntity : EntityBase
{
    public int CityId { get; set; }
    public required string CityCode { get; set; }
    public required string CityName { get; set; }
}
