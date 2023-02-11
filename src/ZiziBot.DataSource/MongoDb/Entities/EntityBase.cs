using MongoDB.Bson;
using MongoFramework.Attributes;

namespace ZiziBot.DataSource.MongoDb.Entities;

public class EntityBase
{
    public ObjectId Id { get; set; }
    [ExtraElements]
    public Dictionary<string, object> Extras { get; set; }
    public int Status { get; set; }
    public string TransactionId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }
}