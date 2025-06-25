using ZiziBot.Common.Enums;

namespace ZiziBot.Common.Interfaces;

public interface IApiKeyService
{
    public Task<string> GetApiKeyAsync(ApiKeyCategory category, ApiKeyVendor name);
}