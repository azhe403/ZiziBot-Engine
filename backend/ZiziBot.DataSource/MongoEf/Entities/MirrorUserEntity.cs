using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("MirrorUser")]
public class MirrorUserEntity : EntityBase
{
    public long UserId { get; set; }
    public DateTime ExpireDate { get; set; }

    [NotMapped]
    public TimeSpan Duration => ExpireDate - DateTime.UtcNow;
}