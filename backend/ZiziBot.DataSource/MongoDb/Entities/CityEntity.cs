namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("City")]
public class CityEntity : EntityBase
{
    public int CityId { get; set; }
    public string CityName { get; set; }
    public bool EnableNotification { get; set; }
    public long UserId { get; set; }
    public long ChatId { get; set; }
}