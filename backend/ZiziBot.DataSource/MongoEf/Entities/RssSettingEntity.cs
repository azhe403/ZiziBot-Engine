using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("RssHistory")]
public class RssSettingEntity : EntityBase
{
    public long ChatId { get; set; }
    public int ThreadId { get; set; }
    public long UserId { get; set; }
    public string RssUrl { get; set; }
    public string CronJobId { get; set; }
    public DateTime LastSuccessDate { get; set; }
    public string? LastErrorMessage { get; set; }
}