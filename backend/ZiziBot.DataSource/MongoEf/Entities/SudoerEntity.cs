using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("Sudoer")]
public class SudoerEntity : EntityBase
{
    public long UserId { get; set; }
    public long PromotedBy { get; set; }
    public long PromotedFrom { get; set; }
}