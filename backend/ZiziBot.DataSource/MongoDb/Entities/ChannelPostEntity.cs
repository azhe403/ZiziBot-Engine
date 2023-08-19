namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("ChannelPost")]
public class ChannelPostEntity : EntityBase
{
    public long SourceChannelId { get; set; }
    public long SourceMessageId { get; set; }
    public long DestinationChatId { get; set; }
    public long DestinationThreadId { get; set; }
    public long DestinationMessageId { get; set; }
}