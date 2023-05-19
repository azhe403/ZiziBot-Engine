namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("BotUser")]
public class BotUserEntity : EntityBase
{
    public long UserId { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? LanguageCode { get; set; }
}