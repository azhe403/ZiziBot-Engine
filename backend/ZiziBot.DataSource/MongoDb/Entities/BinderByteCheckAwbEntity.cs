namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("BinderByte.CheckAwb")]
public class BinderByteCheckAwbEntity : EntityBase
{
    public string Awb { get; set; }
    public string Courier { get; set; }
    public AwbInfo AwbInfo { get; set; }
}