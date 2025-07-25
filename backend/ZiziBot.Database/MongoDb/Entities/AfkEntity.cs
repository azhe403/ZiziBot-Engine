using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("Afk")]
public class AfkEntity : EntityBase
{
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public string? Reason { get; set; }
    public bool IsAfk { get; set; }
}