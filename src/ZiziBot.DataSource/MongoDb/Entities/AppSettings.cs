using MongoDB.Bson;

namespace ZiziBot.DataSource.MongoDb.Entities;

public class AppSettings
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Field { get; set; }
    public string Value { get; set; }
    public string DefaultValue { get; set; }
}