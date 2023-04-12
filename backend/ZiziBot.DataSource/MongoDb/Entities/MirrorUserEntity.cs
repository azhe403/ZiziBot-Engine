namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("MirrorUser")]
public class MirrorUserEntity : EntityBase
{
    public long UserId { get; set; }
    public DateTime ExpireDate { get; set; }

    [NotMapped]
    public TimeSpan Duration => ExpireDate - DateTime.UtcNow;
}