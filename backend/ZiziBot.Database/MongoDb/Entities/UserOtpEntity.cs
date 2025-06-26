namespace ZiziBot.Database.MongoDb.Entities;

public class UserOtpEntity : EntityBase
{
    public long UserId { get; set; }
    public int Otp { get; set; }
}