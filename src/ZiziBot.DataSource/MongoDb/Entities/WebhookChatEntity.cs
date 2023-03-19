using System.ComponentModel.DataAnnotations.Schema;

namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("WebhookChat")]
public class WebhookChatEntity : EntityBase
{
    public long ChatId { get; set; }
    public int ThreadId { get; set; }
    public string RouteId { get; set; }
}