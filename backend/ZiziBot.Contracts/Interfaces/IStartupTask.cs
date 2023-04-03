namespace ZiziBot.Contracts.Interfaces;

public interface IStartupTask
{
    public bool SkipAwait { get; set; }

    Task ExecuteAsync();
}