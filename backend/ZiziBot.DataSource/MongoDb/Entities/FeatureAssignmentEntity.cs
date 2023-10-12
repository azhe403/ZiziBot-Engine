namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("FeatureAssignment")]
public class FeatureAssignmentEntity : EntityBase
{
    public long ChatId { get; set; }
    public int RoleId { get; set; }
}