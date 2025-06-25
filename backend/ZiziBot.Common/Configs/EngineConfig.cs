using System.ComponentModel;
using ZiziBot.TelegramBot.Framework.Models.Enums;
using ExecutionStrategy = ZiziBot.TelegramBot.Framework.Models.Enums.ExecutionStrategy;

namespace ZiziBot.Common.Configs;

[DisplayName("Engine")]
public class EngineConfig
{
    public string ProductName { get; set; }
    public string Description { get; set; }
    public string Vendor { get; set; }
    public string Website { get; set; }
    public string Support { get; set; }
    public BotEngineMode TelegramEngineMode { get; set; }
    public ExecutionStrategy ExecutionStrategy { get; set; }

    public bool EnableChatRestriction { get; set; }
}