using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("PendekinMap")]
public class PendekinMapEntity : EntityBase
{
    public string OriginalUrl { get; set; }
    public string ShortPath { get; set; }
}