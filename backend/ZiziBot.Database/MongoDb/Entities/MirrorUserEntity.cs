using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("MirrorUser")]
public class MirrorUserEntity : EntityBase
{
    public long UserId { get; set; }
    public DateTime ExpireDate { get; set; }

    [NotMapped]
    public TimeSpan Duration => ExpireDate - DateTime.UtcNow;
}