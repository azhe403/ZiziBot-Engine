using Newtonsoft.Json;

namespace ZiziBot.Contracts.Vendor.FathimahApi;

public class CityResponse
{
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("query")]
    public Query Query { get; set; }

    [JsonProperty("kota")]
    public List<Kota> Kota { get; set; }
}

public class Kota
{
    [JsonProperty("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonProperty("nama")]
    public string Nama { get; set; }
}