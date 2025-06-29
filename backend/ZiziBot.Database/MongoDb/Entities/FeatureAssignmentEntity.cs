using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("FeatureAssignment")]
public class FeatureAssignmentEntity : EntityBase
{
    public required string RolloutId { get; set; }
    public long ChatId { get; set; }
}