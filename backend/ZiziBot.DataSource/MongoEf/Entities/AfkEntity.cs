using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("Afk")]
public class AfkEntity : EntityBase
{
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public string? Reason { get; set; }
    public bool IsAfk { get; set; }
}