using JetBrains.Annotations;

namespace ZiziBot.Interfaces;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IUserService
{
    Task<(string stringToken, DateTime tokenExpiration, int accessExpireIn)> GenerateAccessToken(long userId);
}