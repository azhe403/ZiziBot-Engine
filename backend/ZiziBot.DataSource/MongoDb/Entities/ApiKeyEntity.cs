namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("ApiKey")]
public class ApiKeyEntity : EntityBase
{
    public string Name { get; set; }
    public string ApiKey { get; set; }
    public string Category { get; set; }
}