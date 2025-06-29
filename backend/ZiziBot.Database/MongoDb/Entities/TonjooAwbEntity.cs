using MongoDB.EntityFrameworkCore;
using ZiziBot.Common.Vendor.TonjooStudio;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("Tonjoo.Awb")]
public class TonjooAwbEntity : EntityBase
{
    public string Awb { get; set; }
    public string Courier { get; set; }
    public TonjooAwbDetail Detail { get; set; }
}