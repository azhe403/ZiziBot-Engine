namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("Afk")]
public class AfkEntity : EntityBase
{
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public string? Reason { get; set; }
    public bool IsAfk { get; set; }
}