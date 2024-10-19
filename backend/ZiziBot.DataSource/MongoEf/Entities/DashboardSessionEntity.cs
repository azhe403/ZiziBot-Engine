using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("DashboardSession")]
public class DashboardSessionEntity : EntityBase
{
    public long TelegramUserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? PhotoUrl { get; set; }
    public long AuthDate { get; set; }
    public string Hash { get; set; }
    public string SessionId { get; set; }
    public string BearerToken { get; set; }
}