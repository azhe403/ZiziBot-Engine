using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("BotUser")]
public class BotUserEntity : EntityBase
{
    public long UserId { get; set; }
    public string? Username { get; set; }
    public string? ProfilePhotoId { get; set; }
    public string? ProfilePhotoPath { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? LanguageCode { get; set; }
}