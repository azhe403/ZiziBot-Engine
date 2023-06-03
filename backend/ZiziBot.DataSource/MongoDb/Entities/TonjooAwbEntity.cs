namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("Tonjoo.Awb")]
public class TonjooAwbEntity : EntityBase
{
    public string Awb { get; set; }
    public string Courier { get; set; }
    public TonjooAwbDetail Detail { get; set; }
}