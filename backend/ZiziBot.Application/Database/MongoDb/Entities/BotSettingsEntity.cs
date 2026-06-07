using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Application.Database.MongoDb.Entities;

[Collection("BotSettings")]
public class BotSettingsEntity : EntityBase
{
    public string Name { get; set; }
    public string Token { get; set; }
}
