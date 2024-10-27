using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("BotSettings")]
public class BotSettingsEntity : EntityBase
{
    public string Name { get; set; }
    public string Token { get; set; }
}