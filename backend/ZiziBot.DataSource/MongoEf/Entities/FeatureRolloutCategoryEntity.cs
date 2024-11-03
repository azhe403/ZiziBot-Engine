using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("FeatureRolloutCategory")]
public class FeatureRolloutCategoryEntity : EntityBase
{
    public required string Name { get; set; }
}