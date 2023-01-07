namespace ZiziBot.DataSource.MongoDb.Entities;

public class DashboardSession : EntityBase
{
    public long TelegramUserId { get; set; }
    public string FirstName { get; set; }
    public string Username { get; set; }
    public string PhotoUrl { get; set; }
    public long AuthDate { get; set; }
    public string Hash { get; set; }
    public string SessionId { get; set; }
}