using System.ComponentModel.DataAnnotations.Schema;

namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("RssSetting")]
public class RssSettingEntity : EntityBase
{
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public string RssUrl { get; set; }
}