using System.ComponentModel;

namespace ZiziBot.Common.Configs;

[DisplayName("EventLog")]
public class EventLogConfig
{
    public long ChatId { get; set; }
    public int ThreadId { get; set; }
    public int BackupDB { get; set; }
    public int Exception { get; set; }
    public int EventLog { get; set; }

    public bool ProcessEnrich { get; set; }
    public bool WriteToFile { get; set; }
    public bool WriteToSignalR { get; set; }
    public bool WriteToTelegram { get; set; }
}