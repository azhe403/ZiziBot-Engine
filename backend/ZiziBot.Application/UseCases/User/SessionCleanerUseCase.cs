using Microsoft.EntityFrameworkCore;

namespace ZiziBot.Application.UseCases.User;

public class SessionCleanerUseCase(DataFacade dataFacade)
{
    public async Task Handle()
    {
        var sessions = await dataFacade.MongoEf.DashboardSessions
            .Where(x => x.ExpireDate <= DateTime.UtcNow)
            .ToListAsync();

        sessions.ForEach(x => {
            dataFacade.MongoEf.DashboardSessions.Remove(x);
        });

        await dataFacade.MongoEf.SaveChangesAsync();
    }
}