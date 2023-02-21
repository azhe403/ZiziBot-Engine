namespace ZiziBot.DataSource.MongoDb.Entities;

public class BotSettings : EntityBase
{
    public string Name { get; set; }
    public string Token { get; set; }
}