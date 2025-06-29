using System.ComponentModel;

namespace ZiziBot.Common.Configs;

[DisplayName("Hangfire")]
public class HangfireConfig
{
    public string DashboardTitle { get; set; }
    public CurrentStorage CurrentStorage { get; set; }
    public string MongoDbConnection { get; set; }
    public int WorkerMultiplier { get; set; } = 2;
    public string Queues { get; set; } = "default";
}

public enum CurrentStorage
{
    InMemory,
    MongoDb,
    Sqlite,
    LiteDb
}