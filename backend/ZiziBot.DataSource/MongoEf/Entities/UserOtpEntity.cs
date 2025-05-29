namespace ZiziBot.DataSource.MongoEf.Entities;

public class UserOtpEntity : EntityBase
{
    public long UserId { get; set; }
    public int Otp { get; set; }
}