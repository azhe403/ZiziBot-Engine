using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("GlobalBan")]
public class GlobalBanEntity : EntityBase
{
    public long UserId { get; set; }
    public long ChatId { get; set; }
    public string Reason { get; set; }
}