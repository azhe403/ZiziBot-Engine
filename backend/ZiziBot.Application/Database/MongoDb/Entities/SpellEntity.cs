using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Application.Database.MongoDb.Entities;

[Collection("Spell")]
public class SpellEntity : EntityBase
{
    public required string Pattern { get; set; }
    public required string Replacement { get; set; }
}
