namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("Sudoer")]
public class SudoerEntity : EntityBase
{
    public long UserId { get; set; }
    public long PromotedBy { get; set; }
    public long PromotedFrom { get; set; }
}