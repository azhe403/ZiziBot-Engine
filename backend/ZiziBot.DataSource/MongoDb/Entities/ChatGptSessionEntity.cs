namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("ChatGptSession")]
public class ChatGptSessionEntity : EntityBase
{
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public string SessionId { get; set; }
    public string Question { get; set; }
    public DateTime ExpireDate { get; set; }
}