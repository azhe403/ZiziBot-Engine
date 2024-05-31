using MongoDB.Bson;

namespace ZiziBot.DataSource.MongoEf.Entities;

public class EntityBase
{
    public ObjectId Id { get; set; }
    public required EventStatus Status { get; set; }
    public required string TransactionId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}