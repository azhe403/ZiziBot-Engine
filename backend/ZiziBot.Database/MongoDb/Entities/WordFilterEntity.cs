using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("WordFilter")]
public class WordFilterEntity : EntityBase
{
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public string Word { get; set; }
    public bool IsGlobal { get; set; }
    public bool IsRegex { get; set; }
    public PipelineResultAction[]? Action { get; set; }
}