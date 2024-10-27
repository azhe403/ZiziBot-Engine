using Data = ZiziBot.Types.Vendor.BinderByte.Data;

namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("BinderByte.CheckAwb")]
public class BinderByteCheckAwbEntity : EntityBase
{
    public string Awb { get; set; }
    public string Courier { get; set; }
    public Data AwbInfo { get; set; }
}