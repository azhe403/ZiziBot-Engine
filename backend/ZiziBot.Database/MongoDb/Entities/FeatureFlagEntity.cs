using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("FeatureFlag")]
public class FeatureFlagEntity : EntityBase
{
    public required string Name { get; set; }
    public bool IsEnabled { get; set; }
}