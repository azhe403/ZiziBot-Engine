namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("AppSettings")]
public class AppSettingsEntity : EntityBase
{
    public string Name { get; set; }
    public string Field { get; set; }
    public string Value { get; set; }
    public string DefaultValue { get; set; }
}