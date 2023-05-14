namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("WebhookChat")]
public class WebhookChatEntity : EntityBase
{
    public string RouteId { get; set; }
    public long ChatId { get; set; }
    public int MessageThreadId { get; set; }
}