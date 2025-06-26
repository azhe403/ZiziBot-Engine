using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("WebhookChat")]
public class WebhookChatEntity : EntityBase
{
    public string RouteId { get; set; }
    public long ChatId { get; set; }
    public int? MessageThreadId { get; set; }
}