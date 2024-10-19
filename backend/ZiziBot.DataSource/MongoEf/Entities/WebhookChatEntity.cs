using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("WebhookChat")]
public class WebhookChatEntity : EntityBase
{
    public string RouteId { get; set; }
    public long ChatId { get; set; }
    public int MessageThreadId { get; set; }
}