using Microsoft.EntityFrameworkCore;
using ZiziBot.Application.Database.MongoDb;
using ZiziBot.Application.Database.MongoDb.Entities;

namespace ZiziBot.Application.Database.Repository;

public class OutboxRepository(MongoDbContext mongoDbContext)
{
    public async Task EnqueueAsync(OutboxMessageEntity message, CancellationToken cancellationToken = default)
    {
        await mongoDbContext.OutboxMessages.AddAsync(message, cancellationToken);
    }

    public async Task<List<OutboxMessageEntity>> GetPendingAsync(int take = 100, CancellationToken cancellationToken = default)
    {
        return await mongoDbContext.OutboxMessages.AsNoTracking()
            .Where(x => x.Status == EventStatus.Complete)
            .Where(x => x.ProcessedAt == null)
            .OrderBy(x => x.OccurredAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
