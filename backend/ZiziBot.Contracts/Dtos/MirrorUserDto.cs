namespace ZiziBot.Contracts.Dtos
{
    public class MirrorUserDto
    {
        public int UserId { get; set; }
        public DateTime ExpireAt { get; set; }
        public int AddDays { get; set; }
    }
}