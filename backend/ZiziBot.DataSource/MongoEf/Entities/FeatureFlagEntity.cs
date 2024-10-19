using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("FeatureFlag")]
public class FeatureFlagEntity : EntityBase
{
    public required string Name { get; set; }
    public bool IsEnabled { get; set; }
}