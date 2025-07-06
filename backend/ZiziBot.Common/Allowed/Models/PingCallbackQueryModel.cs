using System.Text.Json.Serialization;

namespace ZiziBot.Common.Allowed.Models;

public class PingCallbackQueryModel
{
    [JsonPropertyName("webhookInfo")]
    public string Data { get; set; }
}