namespace ZiziBot.DataSource.MongoDb.Entities;

public class AppSettings : EntityBase
{
	public string Name { get; set; }
	public string Field { get; set; }
	public string Value { get; set; }
	public string DefaultValue { get; set; }
}