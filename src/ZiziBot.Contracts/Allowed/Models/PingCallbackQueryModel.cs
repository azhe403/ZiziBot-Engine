using System.Text.Json.Serialization;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Models;

public class PingCallbackQueryModel : CallbackQueryModel
{
    [JsonPropertyName("webhookInfo")]
    public string Data { get; set; }
}