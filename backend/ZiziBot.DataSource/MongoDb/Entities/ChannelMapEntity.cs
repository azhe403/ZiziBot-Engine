namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("ChannelMap")]
public class ChannelMapEntity : EntityBase
{
    public long ChannelId { get; set; }
    public long ChatId { get; set; }
    public long ThreadId { get; set; }
}