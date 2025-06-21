using JetBrains.Annotations;

namespace ZiziBot.Contracts.Interfaces;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IStartupTask
{
    Task ExecuteAsync();
}