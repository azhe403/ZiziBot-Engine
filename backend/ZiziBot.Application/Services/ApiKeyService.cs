using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Services;

public class ApiKeyService
{
    private readonly ILogger<ApiKeyService> _logger;
    private readonly MongoDbContextBase _mongoDbContext;

    public ApiKeyService(ILogger<ApiKeyService> logger, MongoDbContextBase mongoDbContext)
    {
        _logger = logger;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<ApiKeyEntity?> GetApiKeyAsync(string category, string name)
    {
        var apiKey = await _mongoDbContext.ApiKey
            .FirstOrDefaultAsync(entity =>
                entity.Category == category &&
                entity.Name == name &&
                entity.Status == (int)EventStatus.Complete
            );

        return apiKey;
    }

    public async Task<List<ApiKeyEntity>?> GetApiKeysAsync(string category, string name)
    {
        var apiKey = await _mongoDbContext.ApiKey
            .Where(entity =>
                entity.Category == category &&
                entity.Name == name &&
                entity.Status == (int)EventStatus.Complete
            ).ToListAsync();

        return apiKey;
    }
}