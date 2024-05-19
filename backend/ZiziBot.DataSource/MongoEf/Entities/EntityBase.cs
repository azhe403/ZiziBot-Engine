using MongoDB.Bson;
using MongoFramework.Attributes;

namespace ZiziBot.DataSource.MongoEf.Entities;

public class EntityBase
{
    public ObjectId Id { get; set; }

    [ExtraElements]
    public Dictionary<string, object> Extras { get; set; }

    public EventStatus Status { get; set; }
    public required string TransactionId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}