using MongoDB.EntityFrameworkCore;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Collection("Tonjoo.Awb")]
public class TonjooAwbEntity : EntityBase
{
    public string Awb { get; set; }
    public string Courier { get; set; }
    public TonjooAwbDetail Detail { get; set; }
}