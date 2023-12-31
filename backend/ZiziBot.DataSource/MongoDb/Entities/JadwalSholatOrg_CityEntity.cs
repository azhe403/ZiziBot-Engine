namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("JadwalSholatOrg.City")]
public class JadwalSholatOrg_CityEntity : EntityBase
{
    public int CityId { get; set; }
    public string CityCode { get; set; }
    public string CityName { get; set; }
}