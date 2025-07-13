using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("RssHistory")]
public class RssHistoryEntity : EntityBase
{
    public long ChatId { get; set; }
    public int? ThreadId { get; set; }
    public required string RssUrl { get; set; }
    public required string Url { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public DateTime PublishDate { get; set; }
}