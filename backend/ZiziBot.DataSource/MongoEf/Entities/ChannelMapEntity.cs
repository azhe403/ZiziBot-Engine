using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("ChannelMap")]
public class ChannelMapEntity : EntityBase
{
    public long ChannelId { get; set; }
    public long ChatId { get; set; }
    public long ThreadId { get; set; }
}