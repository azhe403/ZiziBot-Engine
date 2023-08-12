using System.ComponentModel;

namespace ZiziBot.Contracts.Configs;

[DisplayName("Engine")]
public class EngineConfig
{
    public string ProductName { get; set; }
    public string Description { get; set; }
    public string Vendor { get; set; }
    public string Website { get; set; }
    public string Support { get; set; }
    public BotEngineMode TelegramEngineMode { get; set; }

    public bool EnableChatRestriction { get; set; }
}