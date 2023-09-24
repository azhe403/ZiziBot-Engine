using System.ComponentModel;

namespace ZiziBot.Contracts.Configs;

[DisplayName("OptiicDev")]
public class OptiicDevConfig
{
    public string ApiKey { get; set; }
    public List<string> ApiKeys => ApiKey.Split(",").Select(x => x.Trim()).ToList();
}