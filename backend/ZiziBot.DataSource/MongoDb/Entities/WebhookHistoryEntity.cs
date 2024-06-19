namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("WebhookHistory")]
public class WebhookHistoryEntity : EntityBase
{
    public string RouteId { get; set; }
    public long ChatId { get; set; }
    public int MessageId { get; set; }
    public int MessageThreadId { get; set; }
    public WebhookSource WebhookSource { get; set; }
    public string? Header { get; set; }
    public string EventName { get; set; }
    public string? Payload { get; set; }
    public TimeSpan Elapsed { get; set; }
}