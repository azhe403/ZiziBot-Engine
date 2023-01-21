namespace ZiziBot.Contracts.Configs;

public class OptiicDevConfig
{
    public string ApiKey { get; set; }
    public List<string> ApiKeys => ApiKey.Split(",").TrimEach().ToList();
}