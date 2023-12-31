namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("JadwalSholatOrg.Schedule")]
public class JadwalSholatOrg_ScheduleEntity : EntityBase
{
    public int CityId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Fajr { get; set; }
    public TimeOnly Sunrise { get; set; }
    public TimeOnly Dhuhr { get; set; }
    public TimeOnly Ashr { get; set; }
    public TimeOnly Maghrib { get; set; }
    public TimeOnly Isha { get; set; }
}