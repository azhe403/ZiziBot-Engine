namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("BotSettings")]
public class BotSettingsEntity : EntityBase
{
    public string Name { get; set; }
    public string Token { get; set; }
}