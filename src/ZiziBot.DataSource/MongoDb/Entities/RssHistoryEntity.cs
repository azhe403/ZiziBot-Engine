using System.ComponentModel.DataAnnotations.Schema;

namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("RssHistory")]
public class RssHistoryEntity : EntityBase
{
    public long ChatId { get; set; }
    public string RssUrl { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTime PublishDate { get; set; }
}