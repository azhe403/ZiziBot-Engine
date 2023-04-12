namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("WelcomeMessage")]
public class WelcomeMessageEntity : EntityBase
{
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public string Text { get; set; }
    public string RawButton { get; set; }
    public string Media { get; set; }
    public int DataType { get; set; }
}