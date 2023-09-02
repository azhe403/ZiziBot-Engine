namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("WarnUser")]
public class WarnUserEntity : EntityBase
{
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public int Step { get; set; }
    public int WarnType { get; set; }
    public string Reason { get; set; }
}