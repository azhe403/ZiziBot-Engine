using JetBrains.Annotations;

namespace ZiziBot.Common.Interfaces;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IStartupTask
{
    Task ExecuteAsync();
}