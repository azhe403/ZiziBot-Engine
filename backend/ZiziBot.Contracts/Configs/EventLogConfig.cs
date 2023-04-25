using System.ComponentModel;

namespace ZiziBot.Contracts.Configs;

[DisplayName("EventLog")]
public class EventLogConfig
{
    public long ChatId { get; set; }
    public int ThreadId { get; set; }
    public int BackupDB { get; set; }
    public int Exception { get; set; }
    public int EventLog { get; set; }
}