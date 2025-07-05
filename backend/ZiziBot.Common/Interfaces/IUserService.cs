using JetBrains.Annotations;

namespace ZiziBot.Common.Interfaces;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IUserService
{
    Task<(string stringToken, DateTime tokenExpiration, int accessExpireIn)> GenerateAccessToken(long userId);
}