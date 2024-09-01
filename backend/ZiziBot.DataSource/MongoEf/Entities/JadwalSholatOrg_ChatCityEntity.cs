namespace ZiziBot.DataSource.MongoEf.Entities;

[Table("JadwalSholatOrg.ChatCity")]
public class JadwalSholatOrg_ChatCityEntity : EntityBase
{
    public int CityId { get; set; }
    public int ChatId { get; set; }
}