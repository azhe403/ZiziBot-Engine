namespace ZiziBot.DataSource.MongoDb.Entities;

public class Sudoer: EntityBase
{
    public long UserId { get; set; }
    public long PromotedBy { get; set; }
    public long PromotedFrom { get; set; }
}