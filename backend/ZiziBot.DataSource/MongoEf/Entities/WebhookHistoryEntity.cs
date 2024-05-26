using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("WebhookHistory")]
public class WebhookHistoryEntity : EntityBase
{
    public required string RouteId { get; set; }
    public required long ChatId { get; set; }
    public required int MessageId { get; set; }
    public int MessageThreadId { get; set; }
    public required WebhookSource WebhookSource { get; set; }
    public required string Payload { get; set; }
}