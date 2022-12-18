namespace ZiziBot.Hangfire;

public class HangfireAuthorization : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}