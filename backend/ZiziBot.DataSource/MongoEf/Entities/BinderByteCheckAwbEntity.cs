using ZiziBot.Types.Vendor.BinderByte;

namespace ZiziBot.DataSource.MongoEf.Entities;

[Table("BinderByte.CheckAwb")]
public class BinderByteCheckAwbEntity : EntityBase
{
    public string Awb { get; set; }
    public string Courier { get; set; }
    public Data AwbInfo { get; set; }
}