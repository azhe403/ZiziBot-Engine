namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("FeatureRole")]
public class FeatureRoleEntity : EntityBase
{
    public int RoleId { get; set; }
    public required string Name { get; set; }
}