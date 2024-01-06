using System.Text.Json.Serialization;

namespace ZiziBot.Types.Vendor.FathimahApi.v2;

public class CityResponse
{
    public bool Status { get; set; }
    public RequestData Request { get; set; }

    [JsonPropertyName("data")]
    public CityData[] Cities { get; set; }
}