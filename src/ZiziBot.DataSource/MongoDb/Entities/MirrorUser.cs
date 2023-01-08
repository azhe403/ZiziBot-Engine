namespace ZiziBot.DataSource.MongoDb.Entities
{
    public class MirrorUser : EntityBase
    {
        public long UserId { get; set; }
        public DateTime ExpireAt { get; set; }
        public int AddDays { get; set; }
    }
}