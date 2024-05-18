using ZiziBot.Contracts.Enums;

namespace ZiziBot.Interfaces;

public interface IApiKeyService
{
    public Task<string> GetApiKeyAsync(ApiKeyCategory category, ApiKeyVendor name);
}