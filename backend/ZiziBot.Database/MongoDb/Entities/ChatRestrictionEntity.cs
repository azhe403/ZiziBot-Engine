using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("ChatRestriction")]
public class ChatRestrictionEntity : EntityBase
{
    public long UserId { get; set; }
    public long ChatId { get; set; }
}