using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("ApiKey")]
public class ApiKeyEntity : EntityBase
{
    public ApiKeyVendor Name { get; set; }
    public ApiKeyCategory Category { get; set; }
    public string ApiKey { get; set; }
    public DateTime? LastUsedDate { get; set; }
    public string? Note { get; set; }
}