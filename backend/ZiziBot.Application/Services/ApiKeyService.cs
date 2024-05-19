using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using ZiziBot.Interfaces;

namespace ZiziBot.Application.Services;

public class ApiKeyService(
    ILogger<ApiKeyService> logger,
    MongoDbContextBase mongoDbContext
) : IApiKeyService
{
    private readonly ILogger<ApiKeyService> _logger = logger;

    public async Task<string> GetApiKeyAsync(ApiKeyCategory category, ApiKeyVendor name)
    {
        var apiKey = await mongoDbContext.ApiKey
            .OrderBy(entity => entity.LastUsedDate)
            .Where(entity => entity.Category == category)
            .Where(entity => entity.Name == name)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (apiKey != null)
        {
            apiKey.LastUsedDate = DateTime.UtcNow;
            apiKey.TransactionId = Guid.NewGuid().ToString();
            await mongoDbContext.SaveChangesAsync();

            return apiKey.ApiKey;
        }

        return string.Empty;
    }
}