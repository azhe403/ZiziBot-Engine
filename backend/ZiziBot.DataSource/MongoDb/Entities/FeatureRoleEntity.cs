namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("FeatureRole")]
public class FeatureRoleEntity : EntityBase
{
    public required string Name { get; set; }
}