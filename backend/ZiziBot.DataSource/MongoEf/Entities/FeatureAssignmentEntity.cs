using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("FeatureAssignment")]
public class FeatureAssignmentEntity : EntityBase
{
    public required string RolloutId { get; set; }
    public long ChatId { get; set; }
}