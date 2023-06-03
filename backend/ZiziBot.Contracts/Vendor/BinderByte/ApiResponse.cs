using Newtonsoft.Json;

namespace ZiziBot.Contracts.Vendor.BinderByte;

public class ApiResponse
{
    [JsonProperty("status")]
    public long Status { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("data")]
    public AwbInfo AwbInfo { get; set; }
}

public class AwbInfo
{
    [JsonProperty("summary")]
    public Summary Summary { get; set; }

    [JsonProperty("detail")]
    public Detail Detail { get; set; }

    [JsonProperty("history")]
    public List<History> History { get; set; }
}

public class Detail
{
    [JsonProperty("origin")]
    public string Origin { get; set; }

    [JsonProperty("destination")]
    public string Destination { get; set; }

    [JsonProperty("shipper")]
    public string Shipper { get; set; }

    [JsonProperty("receiver")]
    public string Receiver { get; set; }
}

public class History
{
    [JsonProperty("date")]
    public DateTimeOffset Date { get; set; }

    [JsonProperty("desc")]
    public string Desc { get; set; }

    [JsonProperty("location")]
    public string Location { get; set; }
}

public class Summary
{
    [JsonProperty("awb")]
    public string Awb { get; set; }

    [JsonProperty("courier")]
    public string Courier { get; set; }

    [JsonProperty("service")]
    public string Service { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("date")]
    public string Date { get; set; }

    [JsonProperty("desc")]
    public string Desc { get; set; }

    [JsonProperty("amount")]
    public string Amount { get; set; }

    [JsonProperty("weight")]
    public string Weight { get; set; }
}