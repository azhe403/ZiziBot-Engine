using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Application.Database.MongoDb.Entities;

[Collection("FeatureRolloutCategory")]
public class FeatureRolloutCategoryEntity : EntityBase
{
    public required string Name { get; set; }
}
