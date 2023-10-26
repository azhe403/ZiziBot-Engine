using Hangfire.Storage;
using Serilog;

namespace ZiziBot.Hangfire;

public static class HangfireUtil
{
    public static List<RecurringJobDto> GetRssJobs()
    {
        Log.Information("Collecting previous RSS Jobs..");
        var storageConnection = JobStorage.Current.GetConnection();
        var recurringJobs = storageConnection.GetRecurringJobs();
        var rssJobs = recurringJobs.Where(dto => dto.Id.StartsWith("RssJob", StringComparison.InvariantCultureIgnoreCase)).ToList();
        Log.Debug("Found {Count} RSS Jobs", rssJobs.Count);

        return rssJobs;
    }

    public static void RemoveRssJobs()
    {
        var rssJobs = GetRssJobs();

        rssJobs.ForEach(job => {
            Log.Debug("Deleting RSS Job: {Id}", job.Id);
            RecurringJob.RemoveIfExists(job.Id);
        });

        Log.Information("Deleting RSS {Count} Jobs finish..", rssJobs.Count);

    }
}