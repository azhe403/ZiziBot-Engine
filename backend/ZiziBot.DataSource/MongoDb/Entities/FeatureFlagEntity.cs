namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("FeatureFlag")]
public class FeatureFlagEntity : EntityBase
{
    public int FlagId { get; set; }
    public required string Name { get; set; }
}