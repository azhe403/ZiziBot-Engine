namespace ZiziBot.Contracts.Configs;

public class HangfireConfig
{
    public CurrentStorage CurrentStorage { get; set; }
    public string MongoDbConnection { get; set; }
    public int WorkerMultiplier { get; set; } = 2;
    public string Queues { get; set; } = "default";
}

public enum CurrentStorage
{
    InMemory,
    MongoDb
}