namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("GlobalBan")]
public class GlobalBanEntity : EntityBase
{
    public long UserId { get; set; }
    public long ChatId { get; set; }
    public string Reason { get; set; }
}