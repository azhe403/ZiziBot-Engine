using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZiziBot.Common.Enums;
using ZiziBot.Common.Interfaces;
using ZiziBot.Database.MongoDb;

namespace ZiziBot.Services;

public class ApiKeyService(
    ILogger<ApiKeyService> logger,
    MongoDbContext mongoDbContext
) : IApiKeyService
{
    readonly ILogger<ApiKeyService> _logger = logger;

    public async Task<string> GetApiKeyAsync(ApiKeyCategory category, ApiKeyVendor name)
    {
        var apiKey = await mongoDbContext.ApiKey
            .OrderBy(entity => entity.LastUsedDate)
            .Where(entity => entity.Category == category)
            .Where(entity => entity.Name == name)
            .Where(entity => entity.Status == EventStatus.Complete)
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