using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("WebhookHistory")]
public class WebhookHistoryEntity : EntityBase
{
    public required string RouteId { get; set; }
    public long ChatId { get; set; }
    public int MessageId { get; set; }
    public int MessageThreadId { get; set; }
    public WebhookSource WebhookSource { get; set; }
    public string Payload { get; set; }
}