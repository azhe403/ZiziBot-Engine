using System.ComponentModel.DataAnnotations.Schema;

namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("Note")]
public class NoteEntity : EntityBase
{
    public long UserId { get; set; }
    public long ChatId { get; set; }
    public string Query { get; set; }
    public string Content { get; set; }
    public string FileId { get; set; }
    public string RawButton { get; set; }
}