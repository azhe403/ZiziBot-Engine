using System.ComponentModel;

namespace ZiziBot.Common.Configs;

[DisplayName("Log")]
public class LogConfig
{
    public bool ProcessEnrich { get; set; }
    public bool WriteToFile { get; set; }
    public bool WriteToSignalR { get; set; }
}