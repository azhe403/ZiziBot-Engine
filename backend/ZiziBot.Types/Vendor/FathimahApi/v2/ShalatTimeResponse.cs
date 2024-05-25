using System.Text.Json.Serialization;
using Humanizer;

namespace ZiziBot.Types.Vendor.FathimahApi.v2;

public class ShalatTimeResponse
{
    public bool Status { get; set; }
    public RequestData Request { get; set; }

    [JsonPropertyName("data")]
    public CityData Schedule { get; set; }
}

public class RequestData
{
    public string Path { get; set; }
}

public class CityData
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Id { get; set; }

    public string Lokasi { get; set; }
    public string Daerah { get; set; }
    public Jadwal Jadwal { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public Dictionary<string, string> ShalatDict => Jadwal
        .ToDictionary(letterCasing: LetterCasing.Title)
        .Where(pair => pair.Key != "Tanggal" & pair.Key != "Date")
        .ToDictionary(pair => pair.Key, pair => pair.Value);
}

public class Jadwal
{
    public string Tanggal { get; set; }
    public string Imsak { get; set; }
    public string Subuh { get; set; }
    public string Terbit { get; set; }
    public string Dhuha { get; set; }
    public string Dzuhur { get; set; }
    public string Ashar { get; set; }
    public string Maghrib { get; set; }
    public string Isya { get; set; }
    public string Date { get; set; }
}