using MongoDB.Bson;

namespace ZiziBot.DataSource.MongoDb.Entities;

public class BotSettings
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
    public int Status { get; set; }
}