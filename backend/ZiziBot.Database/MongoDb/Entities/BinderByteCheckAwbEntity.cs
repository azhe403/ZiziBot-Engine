using MongoDB.EntityFrameworkCore;
using ZiziBot.Common.Vendor.BinderByte;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("BinderByte.CheckAwb")]
public class BinderByteCheckAwbEntity : EntityBase
{
    public string Awb { get; set; }
    public string Courier { get; set; }
    public Data AwbInfo { get; set; }
}