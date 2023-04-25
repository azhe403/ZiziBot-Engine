using JetBrains.Annotations;

namespace ZiziBot.Contracts.Interfaces;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IStartupTask
{
    public bool SkipAwait { get; set; }

    Task ExecuteAsync();
}