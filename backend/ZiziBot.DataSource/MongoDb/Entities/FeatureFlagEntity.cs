namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("FeatureFlag")]
public class FeatureFlagEntity : EntityBase
{
    public required string Name { get; set; }
    public bool IsEnabled { get; set; }
}