using System.Text.Json.Serialization;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Contracts.Allowed.Models;

public class PingCallbackQueryModel : CallbackQueryModel
{
    [JsonPropertyName("webhookInfo")]
    public string Data { get; set; }
}